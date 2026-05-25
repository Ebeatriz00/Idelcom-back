using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IContactTypeRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(ContactType entity);
        Task<PagedResult<ContactType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ContactType> GetByIdAsync(long contactTypeId);
        Task<bool> UpdateAsync(ContactType entity);
        Task<bool> PatchStatusAsync(long contactTypeId, string status, long usersBy, long businessId);
    }
}
