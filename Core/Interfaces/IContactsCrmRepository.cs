using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IContactsCrmRepository
    {
        Task<bool> ExistsAsync(string contactsName, long businessId, long? exclude = null );
        Task AddAsync(ContactsCrm entity);
        Task<PagedResult<ContactsCrm>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, long clientsId, long? workerId, string? search, int page, int pageSize);
        Task<ContactsCrm> GetByIdAsync(long contactsCrmId);
        Task<bool> UpdateAsync(ContactsCrm entity);
        Task<bool> PatchStatusAsync(long contactsCrmId, string status, long usersBy, long businessId);
    }
}
