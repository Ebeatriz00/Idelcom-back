using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPMConditionRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? pmConditionId = null);
        Task AddAsync(PMCondition entity);
        Task<PagedResult<PMCondition>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PMCondition> GetByIdAsync(long pmConditionId);
        Task<bool> UpdateAsync(PMCondition entity);
        Task<bool> PatchStatusAsync(long pmConditionId, string status, long usersBy, long businessId);
    }
}
