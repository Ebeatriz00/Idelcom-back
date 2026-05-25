using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAreaRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Area entity);
        Task<PagedResult<Area>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetAreaForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Area> GetByIdAsync(long areaId);
        Task<bool> UpdateAsync(Area area);
        Task<bool> PatchStatusAsync(long areaId, string status, long updatedBy, long businessId);
    }
}
