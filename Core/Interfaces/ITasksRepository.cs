using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITasksRepository
    {
        Task<bool> ExistsAsync(string title, long businessId, long? excludeId = null);
        Task AddAsync(Tasks entity);
        Task<PagedResult<Tasks>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedResult<Tasks>> GetAllProjectsAsync(long businessId, string? search, int page, int pageSize, long? opporId);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Tasks> GetByIdAsync(long tasksId);
        Task<bool> UpdateAsync(Tasks entity);
        Task<bool> PatchStatusAsync(long tasksId, string status, long usersBy, long businessId);

        Task<bool> PatchTaskCompletedAsync(string linkToken, long usersBy, long businessId);
        Task<bool> PatchTaskChangeStateAsync(string linkToken, string status, long usersBy, long businessId);
        Task<bool> PatchTaskPriorityStateAsync(string linkToken, string status, long usersBy, long businessId);
        Task<bool> DeleteTaskOpporAsync(string linkToken, string OpporToken);
        Task<bool> DeleteTaskProjectAsync(string linkToken, string ProjectToken );
    }
}
