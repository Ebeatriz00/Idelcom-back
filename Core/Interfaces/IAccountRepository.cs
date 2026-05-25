using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Account entity);
        Task<PagedResult<Account>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Account> GetByIdAsync(long accountId);
        Task<bool> UpdateAsync(Account entity);
        Task<bool> PatchStatusAsync(long accountId, string status, long usersBy, long businessId);
    }
}
