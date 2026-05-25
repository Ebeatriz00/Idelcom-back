using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IConceptsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Concepts entity);
        Task<PagedResult<Concepts>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Concepts> GetByIdAsync(long conceptsId);
        Task<bool> UpdateAsync(Concepts entity);
        Task<bool> PatchStatusAsync(long conceptsId, string status, long usersBy, long businessId);
    }
}
