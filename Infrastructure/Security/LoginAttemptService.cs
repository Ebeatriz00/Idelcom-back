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

        public async Task<bool> IsLockedOutAsync(string key, CancellationToken ct = default)
        {
            var lockedUntilIso = await _cache.GetStringAsync(LockKey(key), ct);
            if (lockedUntilIso is null) return false;
            if (DateTimeOffset.TryParse(lockedUntilIso, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var until))
            {
                return until > DateTimeOffset.UtcNow;
            }
            return true;
        }

        public async Task RegisterFailureAsync(string key, CancellationToken ct = default)
        {
            var fk = FailKey(key);
            var raw = await _cache.GetStringAsync(fk, ct);
            var count = string.IsNullOrEmpty(raw) ? 0 : int.Parse(raw);
            count++;

            await _cache.SetStringAsync(
                fk,
                count.ToString(CultureInfo.InvariantCulture),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opt.WindowMinutes)
                },
                ct
            );

            if (count >= _opt.MaxAttempts)
            {
                var until = DateTimeOffset.UtcNow.AddMinutes(_opt.LockoutMinutes);
                await _cache.SetStringAsync(
                    LockKey(key),                
                    until.ToString("O"),        
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opt.LockoutMinutes)
                    }, ct);

                await _cache.RemoveAsync(fk, ct); 
            }
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
