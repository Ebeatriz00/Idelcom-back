using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IViabilityRepository
    {
        Task<bool> ExistsAsync(string viability, long businessId, long? excludeId = null);
        Task<PagedResult<Viability>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Viability> GetByIdAsync(string LinkToken);
        Task<bool> PatchStatusAsync(string linkToken, string status, long UsersBy, long businessId);
        
    }
}
