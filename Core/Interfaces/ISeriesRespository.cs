using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISeriesRepository
    {
        Task<bool> ExistsAsync(string seriesName, long businessId, long? excludeId = null);
        Task AddAsync(Series entity);
        Task<PagedResult<Series>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetSeriesForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Series> GetByIdAsync(long seriesId);
        Task<bool> UpdateAsync(Series series);
        Task<bool> PatchStatusAsync(long seriesId, string status, long usersBy, long businessId);
    }
}
