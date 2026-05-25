using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITaxAffTypeRepository
    {
        Task<bool> ExistsAsync(string code, string description, long businessId, long? excludeId = null);
        Task AddAsync(TaxAffType entity);
        Task<PagedResult<TaxAffType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetTaxAffTypeForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<TaxAffType> GetByIdAsync(long taxAffTypeId);
        Task<bool> UpdateAsync(TaxAffType taxAffType);
        Task<bool> PatchStatusAsync(long taxAffTypeId, string status, int UsersBy, long businessId);

    }
}
