using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStateOpportunityRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(StateOpportunity entity);
        Task<PagedResult<StateOpportunity>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<StateOpportunity> GetByIdAsync(long stateOpportunityId);
        Task<bool> UpdateAsync(StateOpportunity StateOpportunity);
        Task<bool> PatchStatusAsync(long stateOpportunityId, string status, long UsersBy, long businessId);

    }
}
