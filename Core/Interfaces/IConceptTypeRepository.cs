using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IConceptTypeRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(ConceptType entity);
        Task<PagedResult<ConceptType>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetConceptTypesForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ConceptType> GetByIdAsync(long conceptTypeId);
        Task<bool> UpdateAsync(ConceptType conceptType);
        Task<bool> PatchStatusAsync(long conceptTypeId, string status, int UsersBy, long businessId);
    }
}
