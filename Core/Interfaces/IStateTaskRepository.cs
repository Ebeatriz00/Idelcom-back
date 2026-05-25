using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStateTaskRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(StateTask entity);
        Task<PagedResult<StateTask>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<List<StateTask>> GetSelectAsync(long businessId);
        Task<StateTask> GetByIdAsync(long stateTaskId);
        Task<bool> UpdateAsync(StateTask entity);
        Task<bool> PatchStatusAsync(long stateTaskId, string status, long UsersBy, long businessId);
    }
}
