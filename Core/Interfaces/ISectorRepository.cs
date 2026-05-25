using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISectorRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Sector entity);
        Task<PagedResult<Sector>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Sector> GetByIdAsync(long sectorId);
        Task<bool> UpdateAsync(Sector entity);
        Task<bool> PatchStatusAsync(long sectorId, string status, long usersBy, long businessId);
    }
}
