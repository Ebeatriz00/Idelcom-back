using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IConceptGroupsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(ConceptGroups entity); 
        Task<PagedResult<ConceptGroups>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ConceptGroups> GetByIdAsync(long conceptGroupsId);
        Task<bool> UpdateAsync(ConceptGroups entity);
        Task<bool> PatchStatusAsync(long conceptGroupsId, string status, long usersBy, long businessId);
    }
}
