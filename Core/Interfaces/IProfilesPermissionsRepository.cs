using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProfilesPermissionsRepository
    {
        Task<bool> ExistsAsync(long profilesId, long modulesPermissionsId, long businessId,  long? excludeId = null);
        Task AddAsync(IEnumerable<ProfilesPermissions> entity);
        Task<PagedResult<ProfilesPermissions>> GetAllAsync(long profilesId, long businessId, int page, int pageSize,  string? search = null);
        Task<ProfilesPermissions> GetByIdAsync(long ProfilesPermissionsId);
        Task<bool> UpdateAsync(ProfilesPermissions profilesPermissions);
        Task<bool> PatchStatusAsync(long ProfilesPermissionsId, string status, long UsersBy, long businessId);
    }
}
