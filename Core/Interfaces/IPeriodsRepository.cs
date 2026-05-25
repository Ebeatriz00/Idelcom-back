using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPeriodsRepository
    {
        Task<bool> ExistsAsync(string description, long exerciseId, long businessId, long? excludeId = null);
        Task AddAsync(Periods entity);
        Task<PagedResult<Periods>> GetAllAsync(long businessId, long exercisesId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync( long businessId, string? search, int page, int pageSize);
        Task<Periods> GetByIdAsync(long periodsId);
        Task<bool> UpdateAsync(Periods entity);
        Task<bool> PatchStatusAsync(long periodsId, string status, long usersBy, long businessId);
        Task<bool> PatchBlockStatusAsync(long periodsId, bool indBlock, long usersBy, long businessId);
        Task<bool> HasActivePeriodsAsync(long exercisesId, long businessId);
    }
}
