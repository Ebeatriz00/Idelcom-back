using Application.DTOs.Auth;
using Application.Services.InterfacesServices;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;


namespace Application.Services
{
    public sealed class PermissionService : IAuthPermissionService
    {
        private readonly IMemoryCache _cache;
        private readonly IAuthPermisionRepository _repo;

        public PermissionService(IMemoryCache cache, IAuthPermisionRepository repo)
        {
            _cache = cache;
            _repo = repo;
        }

        private async Task<string> BuildKeyAsync(long userId, long businessId, CancellationToken ct)
        {
            var info = await _repo.GetCacheKeyInfoAsync(userId, businessId, ct);
            return $"perms:p:{info.ProfilesId}:{businessId}";
        }

        public async Task<AuthEffectivePermsDto[]> GetEffectiveSetAsync(long usersId, long businessId, CancellationToken ct = default)
        {
            var key = await BuildKeyAsync(usersId, businessId, ct);

            if (_cache.TryGetValue(key, out AuthEffectivePermsDto[]? cached) && cached is not null)
                return cached;

            var rows = await _repo.GetEffectiveAsync(usersId, businessId, ct);

            var data = rows.Select(r => new AuthEffectivePermsDto
            {
                ModulesId = r.ModulesId,
                ModulesCode = r.ModulesCode,
                PermissionsId = r.PermissionsId,
                PermissionsName = r.PermissionsName
            }).ToArray();

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                .SetPriority(CacheItemPriority.Low)
                .SetSize(data.Length);

            _cache.Set(key, data, options);
            return data;
        }

        
        public async Task<AuthAllowedModulesDto[]> GetAllowedModulesAsync(long usersId, long businessId, CancellationToken ct = default)
        {
            var key = (await BuildKeyAsync(usersId, businessId, ct)).Replace("perms", "mods");

            if (_cache.TryGetValue(key, out AuthAllowedModulesDto[]? cached) && cached is not null)
                return cached;

            var rows = await _repo.GetAllowedModulesAsync(usersId, businessId, ct);

            var data = rows.Select(r => new AuthAllowedModulesDto
            {
                ModulesId = r.ModulesId,
                Code = r.Code,
                Label = r.Label,
                Path = r.Path,
                IconKey = r.IconKey,
                ParentModulesId = r.ParentModulesId,
                ParentId = r.ParentId,
                OrderNo = r.OrderNo
            }).ToArray();

            _cache.Set(key, data, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                .SetPriority(CacheItemPriority.Low)
                .SetSize(data.Length));

            return data;
        }

        public string ComputeHash(IEnumerable<string> items)
        {
            var joined = string.Join("|", items.OrderBy(x => x, StringComparer.Ordinal));
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(joined));
            return Convert.ToHexString(bytes);
        }

        public void Invalidate(long profilesId, long businessId)
        {
            _cache.Remove($"perms:p:{profilesId}:{businessId}");
            _cache.Remove($"mods:p:{profilesId}:{businessId}");
        }
    }
}