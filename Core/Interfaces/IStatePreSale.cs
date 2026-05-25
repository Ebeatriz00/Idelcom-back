using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStatePreSaleRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(StatePreSale entity);
        Task<PagedResult<StatePreSale>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<StatePreSale> GetByIdAsync(long statePreSaleId);
        Task<bool> UpdateAsync(StatePreSale statePreSale);
        Task<bool> PatchStatusAsync(long statePreSaleId, string status, long UsersBy, long businessId);
    }
}
