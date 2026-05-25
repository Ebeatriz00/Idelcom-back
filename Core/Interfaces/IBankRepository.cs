using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBankRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Bank entity);
        Task<PagedResult<Bank>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetBanksForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Bank> GetByIdAsync(long bankId);
        Task<bool> UpdateAsync(Bank entity);
        Task<bool> PatchStatusAsync(long bankId, string status, long usersBy, long businessId);
    }
}
