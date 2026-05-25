using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Logistic
{
    public interface ISupplierGroupsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(SupplierGroups entity);
        Task<PagedResult<SupplierGroups>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<SupplierGroups> GetByIdAsync(long supplierGroupsId);
        Task<bool> UpdateAsync(SupplierGroups supplierGroups);
        Task<bool> PatchStatusAsync(long supplierGroupsId, string status, long usersBy, long businessId);
    }
}
