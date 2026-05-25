using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovVisRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(MovVis entity);
        Task<PagedResult<MovVis>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovVis> GetByIdAsync(long movVisId);
        Task<bool> UpdateAsync(MovVis entity);
        Task<bool> PatchStatusAsync(long movVisId, string status, long usersBy, long businessId);
    }
}
