using Core.Interfaces.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class TokenBlacklist : ITokenBlacklist
    {
        private readonly IDistributedCache _cache;
        public TokenBlacklist(IDistributedCache cache) => _cache = cache;
        private static string Key(string jti) => $"jwt:blacklist:{jti}";
        public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default)
       => await _cache.GetStringAsync(Key(jti), ct) is not null;

        public async Task BlacklistAsync(string jti, TimeSpan ttl, CancellationToken ct = default)
            => await _cache.SetStringAsync(Key(jti), "1",
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
    }
}
