using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthPermisionRepository
    {
        Task<IReadOnlyList<AuthEffectivePerms>> GetEffectiveAsync(long usersId, long businessId, CancellationToken ct = default);
        Task<IReadOnlyList<AuthAllowedModules>> GetAllowedModulesAsync(long usersId, long businessId, CancellationToken ct = default);
        Task<AuthCacheKeyInfo> GetCacheKeyInfoAsync(long usersId, long businessId, CancellationToken ct = default);
    }
}
