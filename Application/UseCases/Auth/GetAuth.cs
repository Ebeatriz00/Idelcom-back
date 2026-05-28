using Application.DTOs.Auth;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;
using System.Security.Cryptography;

namespace Application.UseCases.Auth
{
    public sealed class AuthOptions
    {
        public TimeSpan RefreshLifetime { get; init; } = TimeSpan.FromDays(7);
        public bool PreloadAllowedModules { get; init; } = true;
    }

    public sealed class GetAuth
    {
        private readonly IAuthRepository _repository;
        private readonly IPasswordService _passwords;
        private readonly ITokenService _tokens;
        private readonly ILoginAttemptService _attempts;
        private readonly IValidator<AuthRequestDto> _validator;
        private readonly IRefreshTokenService _refreshTokens;
        private readonly IMapper _mapper;
        private readonly IOptions<AuthOptions> _opt;
        private readonly ILogger<GetAuth> _logger;

        public GetAuth(
            IAuthRepository repository,
            IPasswordService passwords,
            ITokenService tokens,
            ILoginAttemptService attempts,
            IValidator<AuthRequestDto> validator,
            IMapper mapper,
            IRefreshTokenService refreshTokens,
            IOptions<AuthOptions> opt,
            ILogger<GetAuth> logger)
        {
            _repository = repository;
            _passwords = passwords;
            _tokens = tokens;
            _attempts = attempts;
            _validator = validator;
            _mapper = mapper;
            _refreshTokens = refreshTokens;
            _opt = opt;
            _logger = logger;
        }

        public async Task<AuthResponseDto> ExecuteAsync(
            AuthRequestDto dto,
            string clientIp,
            CancellationToken ct = default)
        {
            // 1) Validación
            var vr = await _validator.ValidateAsync(dto, ct);
            if (!vr.IsValid)
            {
                throw new Application.Exceptions.ValidationException(
                    vr.Errors
                        .Select(e => new GlobalErrorDetail(e.ErrorCode ?? "VALIDATION", e.ErrorMessage))
                        .ToList());
            }

            var (lookupKey, inputAttemptKey) = BuildKeys(dto, clientIp);
            var refreshLifetime = _opt.Value.RefreshLifetime;

            // 2) Lock por IP/entrada
            var inputRemaining = await _attempts.IsLockedOutAsync(inputAttemptKey, ct);
            if (inputRemaining.HasValue)
            {
                _logger.LogWarning("Login bloqueado — entrada: {LookupKey} IP: {Ip} — tiempo restante: {Remaining}s",
                    lookupKey, clientIp, (int)inputRemaining.Value.TotalSeconds);
                throw new AuthLockedOutException(retryAfter: inputRemaining.Value);
            }

            // 3) Lookup de usuario (por correo/username/etc.)
            var user = await _repository.AuthenticateAsync(lookupKey, ct);

            // Si no existe o está inactivo → fake hash + registrar intento
            if (user is null || user.Status == "0")
            {
                FakeVerify();
                var r = await _attempts.RegisterFailureAsync(inputAttemptKey, ct);
                _logger.LogWarning("Login fallido — usuario no encontrado — entrada: {LookupKey} IP: {Ip}", lookupKey, clientIp);
                if (r.IsNowLocked)
                {
                    _logger.LogWarning("Cuenta bloqueada — entrada: {LookupKey} IP: {Ip} — duración: {Seconds}s",
                        lookupKey, clientIp, (int)(r.LockoutDuration?.TotalSeconds ?? 0));
                    throw new AuthLockedOutException(retryAfter: r.LockoutDuration);
                }
                throw new AuthInvalidCredentialsException(attemptsRemaining: r.AttemptsBeforeNextLock);
            }

            // Ahora sí tiene sentido armar la key por cuenta
            var accountAttemptKey = BuildAccountKey(user.UsersId, clientIp);

            // 4) Lock por cuenta
            var accountRemaining = await _attempts.IsLockedOutAsync(accountAttemptKey, ct);
            if (accountRemaining.HasValue)
            {
                _logger.LogWarning("Login bloqueado — userId: {UserId} IP: {Ip} — tiempo restante: {Remaining}s",
                    user.UsersId, clientIp, (int)accountRemaining.Value.TotalSeconds);
                throw new AuthLockedOutException(retryAfter: accountRemaining.Value);
            }

            // 5) Verificar password (y SIEMPRE limpiar el DTO en finally)
            bool ok;
            try
            {
                ok = _passwords.VerifyPassword(
                    dto.UsersPassword!,
                    user.UsersPassword,
                    user.UsersSalt);
            }
            finally
            {
                ZeroOut(dto);
            }

            if (!ok)
            {
                var inputResult   = await _attempts.RegisterFailureAsync(inputAttemptKey, ct);
                var accountResult = await _attempts.RegisterFailureAsync(accountAttemptKey, ct);

                await Task.Delay(RandomJitter(100, 300), ct);

                if (inputResult.IsNowLocked || accountResult.IsNowLocked)
                {
                    var lockDuration = inputResult.LockoutDuration ?? accountResult.LockoutDuration;
                    _logger.LogWarning("Cuenta bloqueada tras múltiples fallos — userId: {UserId} IP: {Ip} — duración: {Seconds}s",
                        user.UsersId, clientIp, (int)(lockDuration?.TotalSeconds ?? 0));
                    throw new AuthLockedOutException(retryAfter: lockDuration);
                }

                var attemptsLeft = Math.Min(
                    inputResult.AttemptsBeforeNextLock  ?? int.MaxValue,
                    accountResult.AttemptsBeforeNextLock ?? int.MaxValue);

                _logger.LogWarning("Contraseña incorrecta — userId: {UserId} IP: {Ip} — intentos restantes: {Remaining}",
                    user.UsersId, clientIp, attemptsLeft == int.MaxValue ? (int?)null : attemptsLeft);

                throw new AuthInvalidCredentialsException(
                    attemptsRemaining: attemptsLeft == int.MaxValue ? null : attemptsLeft);
            }

            // 6) Éxito → limpiar locks
            await Task.WhenAll(
                _attempts.ResetAsync(inputAttemptKey, ct),
                _attempts.ResetAsync(accountAttemptKey, ct)
            );
            _logger.LogInformation("Login exitoso — userId: {UserId} businessId: {BusinessId} IP: {Ip}",
                user.UsersId, user.BusinessId, clientIp);
            var sid = Guid.NewGuid().ToString("N");
            // 7) Emitir refresh token
            var refresh = await _refreshTokens.IssueAsync(
                user.UsersId,
                user.BusinessId,
                user.ProfilesName,
                sid,
                clientIp,
                device: null,
                refreshLifetime,
                ct);

            // 8) Emitir JWT
           
            var apiToken = _tokens.Create(
                user.UsersId,
                user.BusinessId,
                user.ProfilesName,
                sid
            );

            // 9) Mapear respuesta
            var response = _mapper.Map<AuthResponseDto>(user);
            response.ApiToken = apiToken;
            response.RefreshToken = refresh;
            response.Status = "1";
            response.Message = "Autenticación exitosa.";

            // 10) Preload de módulos permitidos (cuando lo tengas implementado)
            if (_opt.Value.PreloadAllowedModules)
            {
                // Ejemplo futuro:
                // response.AllowedModules = await _permService
                //    .GetAllowedModulesAsync(user.UsersId, user.BusinessId, ct);
            }

            return response;
        }

        private static (string lookupKey, string attemptKey) BuildKeys(AuthRequestDto dto, string clientIp)
        {
            var raw = (dto.UsersKey ?? string.Empty).Trim();
            var lookupKey = raw.Contains('@')
                ? raw.ToLowerInvariant()
                : raw.ToUpperInvariant();

            var ip = CanonIp(clientIp);
            var attemptKey = Application.Common.Security.AuthKeyBuilder.Build(lookupKey, ip);
            return (lookupKey, attemptKey);
        }

        private static string BuildAccountKey(long userId, string clientIp)
        {
            var ip = CanonIp(clientIp);
            return Application.Common.Security.AuthKeyBuilder.Build($"UID:{userId}", ip);
        }

        private static string CanonIp(string? ip) =>
            (ip ?? "unknown").Trim().Replace("::ffff:", "");

        /// <summary>
        /// Simula el coste de un hash PBKDF2 para evitar timing leaks cuando
        /// el usuario no existe o está inactivo.
        /// Idealmente, usa el mismo coste aproximado que tu algoritmo real.
        /// </summary>
        private static void FakeVerify()
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes("dummy_password");
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            // 100k iteraciones ~ coste decente (ajusta según tu realidad)
            using var pbkdf2 = new Rfc2898DeriveBytes(
                passwordBytes,
                salt,
                100_000,
                HashAlgorithmName.SHA256);

            _ = pbkdf2.GetBytes(32); // tiramos el resultado
        }

        private static void ZeroOut(AuthRequestDto dto)
        {
            try
            {
                if (dto.UsersPassword is null) return;
                dto.UsersPassword = string.Empty;
            }
            catch
            {
                // swallow, no queremos romper el flujo solo por el wipe
            }
        }

        private static TimeSpan RandomJitter(int minMs, int maxMs)
        {
            var r = RandomNumberGenerator.GetInt32(minMs, maxMs + 1);
            return TimeSpan.FromMilliseconds(r);
        }
    }


}
