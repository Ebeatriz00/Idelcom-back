using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILeadsSourcesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(LeadsSources entity);
        Task<PagedResult<LeadsSources>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<LeadsSources> GetByIdAsync(long LeadsSourcesId);
        Task<bool> UpdateAsync(LeadsSources entity);
        Task<bool> PatchStatusAsync(long LeadsSourcesId, string status, long usersBy, long businessId);
    }
}
