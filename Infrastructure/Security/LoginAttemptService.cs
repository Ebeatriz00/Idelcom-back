using Core.Interfaces.Services;
using Core.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Infrastructure.Security
{
    public sealed class LoginAttemptService : ILoginAttemptService
    {
        private readonly IDistributedCache _cache;
        private readonly LoginAttemptOptions _opt;

        public LoginAttemptService(IDistributedCache cache, IOptions<LoginAttemptOptions> opt)
        {
            _cache = cache;
            _opt = opt.Value;
        }

        private static string FailKey(string key) => $"auth:fail:{key}";
        private static string LockKey(string key) => $"auth:lock:{key}";

        public async Task<TimeSpan?> IsLockedOutAsync(string key, CancellationToken ct = default)
        {
            var lockedUntilIso = await _cache.GetStringAsync(LockKey(key), ct);
            if (lockedUntilIso is null) return null;

            if (DateTimeOffset.TryParse(lockedUntilIso, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var until))
            {
                var remaining = until - DateTimeOffset.UtcNow;
                return remaining > TimeSpan.Zero ? remaining : null;
            }

            return null;
        }

        public async Task<LoginAttemptResult> RegisterFailureAsync(string key, CancellationToken ct = default)
        {
            var fk = FailKey(key);
            var raw = await _cache.GetStringAsync(fk, ct);
            var count = string.IsNullOrEmpty(raw) ? 0 : int.Parse(raw);
            count++;

            // El contador persiste toda la ventana sin resetearse entre bloqueos
            // para que los intentos escalen progresivamente.
            await _cache.SetStringAsync(
                fk,
                count.ToString(CultureInfo.InvariantCulture),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opt.WindowMinutes)
                },
                ct);

            // Umbral que aplica al conteo actual (el de mayor valor alcanzado).
            var currentStep = _opt.Steps
                .Where(s => count >= s.AfterAttempts)
                .OrderByDescending(s => s.AfterAttempts)
                .FirstOrDefault();

            if (currentStep is not null)
            {
                var lockoutDuration = TimeSpan.FromSeconds(currentStep.LockoutSeconds);
                var until = DateTimeOffset.UtcNow.Add(lockoutDuration);

                await _cache.SetStringAsync(
                    LockKey(key),
                    until.ToString("O"),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = lockoutDuration
                    },
                    ct);

                return new LoginAttemptResult(IsNowLocked: true, LockoutDuration: lockoutDuration, AttemptsBeforeNextLock: null);
            }

            // Calcula cuántos intentos faltan para el próximo bloqueo.
            var nextStep = _opt.Steps
                .Where(s => s.AfterAttempts > count)
                .OrderBy(s => s.AfterAttempts)
                .FirstOrDefault();

            var attemptsLeft = nextStep is not null ? nextStep.AfterAttempts - count : (int?)null;

            return new LoginAttemptResult(IsNowLocked: false, LockoutDuration: null, AttemptsBeforeNextLock: attemptsLeft);
        }

        public Task ResetAsync(string key, CancellationToken ct = default)
        {
            return Task.WhenAll(
                _cache.RemoveAsync(FailKey(key), ct),
                _cache.RemoveAsync(LockKey(key), ct)
            );
        }
    }
}
