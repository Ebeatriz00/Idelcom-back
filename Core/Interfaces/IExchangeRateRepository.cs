using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<bool> ExistsAsync(DateTime datefxrate, long businessId, long? excludeId = null);
        Task<ExchangeRate> GetLastExchangeRateAsync(long businessId);
        Task AddAsync(ExchangeRate entity);
        Task<PagedResult<ExchangeRate>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetExchangeRateForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ExchangeRate> GetByIdAsync(long fxrateId);
        Task<bool> UpdateAsync(ExchangeRate exchangeRate);
        Task<bool> PatchStatusAsync(long fxrateId, string status, long UsersBy, long businessId);
    }
}
