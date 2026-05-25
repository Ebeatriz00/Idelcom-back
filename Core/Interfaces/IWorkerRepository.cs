    using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWorkerRepository
    {
        Task<bool> ExistsAsync(string WorkerDocument, long businessId, long? excludeId = null);
        Task AddAsync(Worker entity);
        Task<PagedResult<Worker>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetWorkerForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetWorkerSalesSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetWorkerProyectsSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetWorkerOperationsSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Worker> GetByIdAsync(long workerId);
        Task<bool> UpdateAsync(Worker worker);
        Task<bool> PatchStatusAsync(long workerId, string status, long updatedBy, long businessId);
    }
}