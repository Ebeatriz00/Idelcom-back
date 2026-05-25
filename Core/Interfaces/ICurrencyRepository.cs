using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<bool> ExistsAsync(string description, string codeSunat, long businessId, long? excludeId = null);
        Task<string> GetLastCurrencyCodeAsync(long businessId);
        Task AddAsync(Currency entity);
        Task<PagedResult<Currency>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetCurrencyForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Currency> GetByIdAsync(long currencyId);
        Task<bool> UpdateAsync(Currency currency);
        Task<bool> PatchStatusAsync(long currencyId, string status, int UsersBy, long businessId);
    }
}
