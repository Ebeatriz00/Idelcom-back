using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWorkerStatusRepository
    {
        Task AddAsync(WorkerStatus entity);
        Task<PagedResult<WorkerStatus>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<WorkerStatus> GetByIdAsync(long workerStatusId);

        Task<WorkerStatus> GetHomologationByIdAsync(long workerStatusId);
        Task<bool> UpdateAsync(WorkerStatus entity);
        Task<bool> PatchStatusAsync(long workerStatusId, string status, long usersBy, long businessId);
    }
}
