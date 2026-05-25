using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IModulesPermissionsRepository
    {  
            Task<bool> ExistsAsync(long modulesId, long permissionsId, long businessId, long? excludeId = null);
            Task AddAsync(ModulesPermissions entity);
            Task<PagedResult<ModulesPermissions>> GetAllAsync(long businessId, int page, int pageSize, string? search = null);
            Task<ModulesPermissions> GetByIdAsync(long modulesPermissionsId);
            Task<bool> UpdateAsync(ModulesPermissions entity);
            Task<bool> PatchStatusAsync(long modulesPermissionsId, string status, long UsersBy, long businessId);
        }
    }
