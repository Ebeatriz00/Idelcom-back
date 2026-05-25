using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPriorityStateRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(PriorityState entity);
        Task<PagedResult<PriorityState>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<List<PriorityState>> GetSelectAsync(long businessId);
        Task<PriorityState> GetByIdAsync(long priorityStateId);
        Task<bool> UpdateAsync(PriorityState entity);
        Task<bool> PatchStatusAsync(long priorityStateId, string status, long UsersBy, long businessId);
    }
}
