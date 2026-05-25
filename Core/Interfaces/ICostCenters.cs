using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICostCentersRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(CostCenters entity);
        Task<PagedResult<CostCenters>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetCostCentersForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<CostCenters> GetByIdAsync(long costCentersId);
        Task<bool> UpdateAsync(CostCenters costCenters);
        Task<bool> PatchStatusAsync(long costCentersId, string status, int usersBy, long businessId);
    }
}
