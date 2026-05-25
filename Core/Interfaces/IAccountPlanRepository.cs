using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAccountPlanRepository
    {
        Task<bool> ExistsAsync(string code, long businessId, long? excludeId = null);
        Task AddAsync(AccountPlan entity);
        Task<PagedResult<AccountPlan>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetAccountPlanForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<AccountPlan> GetByIdAsync(long accountPlanId);
        Task<bool> UpdateAsync(AccountPlan entity);
        Task<bool> PatchStatusAsync(long accountPlanId, string status, long UsersBy, long businessId);
    }
}
