using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

public class UserSessionService : IUserSessionService
{
    private readonly IDistributedCache _cache;

    public UserSessionService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private static string Key(string sid) => $"session:{sid}";

    public async Task RevokeSessionAsync(string sid, TimeSpan ttl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sid)) return;

        await _cache.SetStringAsync(
            Key(sid),
            "revoked",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            },
            ct);
    }

    public async Task<bool> IsRevokedAsync(string sid, CancellationToken ct = default)
    {
        var value = await _cache.GetStringAsync(Key(sid), ct);
        return value != null;
    }
}