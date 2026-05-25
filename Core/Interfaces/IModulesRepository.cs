using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IModulesRepository
    {
        Task<bool> ExistsAsync(string label, long businessId, long? excludeId = null);
        Task AddAsync(Modules entity);
        Task<List<Modules>> GetAllAsync(long businessId, long parentModulesId, string? search, long? usersId);
        Task<PagedSelect<OptionItem>> GetModulesForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Modules> GetByIdAsync(long moduleId);
        Task<bool> UpdateAsync(Modules module);
        Task<bool> PatchStatusAsync(long modulesId, string status, int updatedBy, long businessId);
    }
}
