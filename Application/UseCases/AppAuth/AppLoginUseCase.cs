using Application.DTOs.AppAuth;
using Application.Exceptions;
using Core.Entities.Security;
using Core.Interfaces;
using Core.Interfaces.Security;
using Core.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.AppAuth
{
    public class AppLoginUseCase(
        IAuthRepository authRepository,
        IPasswordService passwordService,
        ILoginAttemptService loginAttemptService,
        IAppDeviceSessionRepository appDeviceSessionRepository,
        IValidator<AppLoginRequestDto> validator,
        IConfiguration configuration)
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly ILoginAttemptService _attempts = loginAttemptService;
        private readonly IAppDeviceSessionRepository _sessionRepository = appDeviceSessionRepository;
        private readonly IValidator<AppLoginRequestDto> _validator = validator;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AppLoginResponseDto> ExecuteAsync(AppLoginRequestDto request, string clientIp, CancellationToken ct = default)
        {
            // validación inicial de los datos de entrada según las reglas de negocio
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode ?? "VALIDATION", e.ErrorMessage))
                            .ToList();

                throw new AppValidationException(errors);
            }

            // normalización del identificador para asegurar consistencia en la búsqueda
            var lookupKey = request.Username.Trim().ToLower();
            lookupKey = lookupKey.Contains('@') ? lookupKey.ToLowerInvariant() : lookupKey.ToUpperInvariant();

            // definición de clave única para el control de intentos fallidos por ip y usuario
            var attemptKey = $"APP_LOGIN_{lookupKey}_{clientIp}";

            // verificación de políticas de bloqueo para mitigar ataques de fuerza bruta
            var appRemaining = await _attempts.IsLockedOutAsync(attemptKey, ct);
            if (appRemaining.HasValue)
                throw new AuthLockedOutException(retryAfter: appRemaining.Value);

            // recuperación del usuario y validación de su estado de activación
            var user = await _authRepository.AuthenticateAsync(lookupKey, ct);

            if (user is null || user.Status == "0")
            {
                var r = await _attempts.RegisterFailureAsync(attemptKey, ct);
                if (r.IsNowLocked)
                    throw new AuthLockedOutException(retryAfter: r.LockoutDuration);
                throw new AuthInvalidCredentialsException(attemptsRemaining: r.AttemptsBeforeNextLock);
            }

            // verificación criptográfica de la contraseña
            bool isPasswordValid = false;
            try
            {
                isPasswordValid = _passwordService.VerifyPassword(request.Password, user.UsersPassword, user.UsersSalt);
            }
            finally
            {
                // limpieza inmediata del campo sensible para reducir exposición en memoria
                request.Password = string.Empty;
            }

            if (!isPasswordValid)
            {
                var r = await _attempts.RegisterFailureAsync(attemptKey, ct);
                if (r.IsNowLocked)
                    throw new AuthLockedOutException(retryAfter: r.LockoutDuration);
                throw new AuthInvalidCredentialsException(attemptsRemaining: r.AttemptsBeforeNextLock);
            }

            // limpieza del historial de intentos tras una autenticación exitosa
            await _attempts.ResetAsync(attemptKey, ct);

            // preparación de identificadores únicos para la trazabilidad del token y la sesión
            var jti = Guid.NewGuid().ToString("N");
            var sid = Guid.NewGuid().ToString("N");

            // construcción de la carga útil (claims) con información de identidad y contexto
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UsersId.ToString()),
                new(JwtRegisteredClaimNames.Jti, jti),
                new("sid", sid),
                new("bid", user.BusinessId.ToString()),
                new("source", "mobile"),
                new("deviceId", request.DeviceId)
            };

            // configuración de la firma digital del token
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["JwtSetting:Key"]!);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // definición del tiempo de vida del token (30 días para entorno móvil)
            var expires = DateTime.UtcNow.AddDays(30);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSetting:Issuer"],
                audience: _configuration["JwtSetting:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // registro de la sesión vinculada al dispositivo específico en el almacenamiento persistente
            var session = new AppDeviceSession
            {
                UserId = user.UsersId,
                DeviceId = request.DeviceId,
                DeviceName = request.DeviceName,
                DeviceBrand = request.DeviceBrand,
                DeviceVersion = request.DeviceVersion,
                AppVersion = request.AppVersion,
                Jti = jti,
                IsRevoked = false,
            };

            await _sessionRepository.CreateAsync(session);

            // construcción de la respuesta de éxito con datos de perfil necesarios para la app
            return new AppLoginResponseDto
            {
                Status = 1,
                Message = "Autenticación móvil exitosa.",
                AccessToken = accessToken,
                UserId = user.UsersId,
                FullName = $"{user.FullName}".Trim(),
                RoleName = user.ProfilesName ?? string.Empty,
                BusinessId = user.BusinessId,
                UserName = user.UsersName
            };
        }
    }
}
