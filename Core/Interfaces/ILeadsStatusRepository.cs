using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILeadsStatusRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(LeadsStatus entity);
        Task<PagedResult<LeadsStatus>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<LeadsStatus> GetByIdAsync(long LeadsStatusId);
        Task<bool> UpdateAsync(LeadsStatus entity);
        Task<bool> PatchStatusAsync(long LeadsStatusId, string status, long usersBy, long businessId);
    }
}
