using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Security
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IDistributedCache _cache;
        private readonly IUserSessionService _sessionService;
        public RefreshTokenService(IDistributedCache cache, IUserSessionService sessionService)
        {
            _cache = cache;
            _sessionService = sessionService;
        }

        private static string Hash(string token)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
        }

        private static string Key(string token) => $"rt:{Hash(token)}";

        private sealed class StoredRt
        {
            public long UserId { get; set; }
            public long BusinessId { get; set; }
            public string? ProfileName { get; set; }
            public string? Sid { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool Revoked { get; set; }
            public string? ReplacedBy { get; set; }
            public string? Ip { get; set; }
            public string? Device { get; set; }
        }

        public async Task<string> IssueAsync(
            long userId,
            long businessId,
            string profilesName,
            string sid,
            string ip,
            string? device,
            TimeSpan lifetime,
            CancellationToken ct = default)
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var key = Key(raw);

            var data = new StoredRt
            {
                UserId = userId,
                BusinessId = businessId,
                ProfileName = profilesName,
                Sid = sid,
                ExpiresAt = DateTimeOffset.UtcNow.Add(lifetime),
                Revoked = false,
                Ip = ip,
                Device = device
            };

            var json = JsonSerializer.Serialize(data);

            await _cache.SetStringAsync(
                key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = data.ExpiresAt
                },
                ct);

            return raw;
        }

        public async Task<(string accessToken, string refreshToken)> RotateAsync(
            string refreshToken,
            Func<long, long, string, string, string> issueAccessTokenForUser,
            string ip,
            string? device,
            TimeSpan newRefreshLifetime,
            CancellationToken ct = default)
        {
            var k = Key(refreshToken);
            var json = await _cache.GetStringAsync(k, ct);

            if (json is null)
                throw new RefreshTokenExpiredException();

            var stored = JsonSerializer.Deserialize<StoredRt>(json)!;

            if(stored.Revoked)
            {
                await _sessionService.RevokeSessionAsync(stored.Sid!, stored.ExpiresAt - DateTimeOffset.UtcNow, ct);
                throw new RefreshTokenInvalidException("Refresh token reutilizado.");
            }

            if (stored.ExpiresAt <= DateTimeOffset.UtcNow)
                throw new RefreshTokenExpiredException();

            if (string.IsNullOrWhiteSpace(stored.ProfileName))
                throw new RefreshTokenInvalidException("Refresh token sin perfil.");

            if (string.IsNullOrWhiteSpace(stored.Sid))
                throw new RefreshTokenInvalidException("Refresh token sin sid.");

            // Revoca el actual
            stored.Revoked = true;

            await _cache.SetStringAsync(
                k,
                JsonSerializer.Serialize(stored),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = stored.ExpiresAt
                },
                ct);

            // Emite nuevo refresh token
            var newRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var newKey = Key(newRaw);

            stored.ReplacedBy = newKey;

            var newStored = new StoredRt
            {
                UserId = stored.UserId,
                BusinessId = stored.BusinessId,
                ProfileName = stored.ProfileName,
                Sid = stored.Sid,
                ExpiresAt = DateTimeOffset.UtcNow.Add(newRefreshLifetime),
                Revoked = false,
                Ip = ip,
                Device = device
            };

            await _cache.SetStringAsync(
                newKey,
                JsonSerializer.Serialize(newStored),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = newStored.ExpiresAt
                },
                ct);

            await _cache.SetStringAsync(
                k,
                JsonSerializer.Serialize(stored),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = stored.ExpiresAt
                },
                ct);

            var access = issueAccessTokenForUser(
                stored.UserId,
                stored.BusinessId,
                stored.ProfileName,
                stored.Sid
            );

            return (access, newRaw);
        }

        public async Task RevokeAsync(string refreshToken, string reason, CancellationToken ct = default)
        {
            var k = Key(refreshToken);
            var json = await _cache.GetStringAsync(k, ct);

            if (json is null)
                return;

            var stored = JsonSerializer.Deserialize<StoredRt>(json)!;
            stored.Revoked = true;

            await _cache.SetStringAsync(
                k,
                JsonSerializer.Serialize(stored),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = stored.ExpiresAt
                },
                ct);
        }
    }
}