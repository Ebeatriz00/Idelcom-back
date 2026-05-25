using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IParentModulesRepository
    {
        Task<bool> ExistsAsync(string title, long businessId, long? excludeId = null);
        Task AddAsync(ParentModules entity);
        Task<PagedResult<ParentModules>> GetAllAsync(int businessId, string? search,int page, int pageSize);
        Task<ParentModules> GetByIdAsync(long parentModuleId);
        Task<bool> UpdateAsync(ParentModules pModules);
        Task<bool> PatchStatusAsync(long parentModulesId, string status, int updatedBy, long businessId);

    }
}
