using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPMVisRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? pmVisId = null);
        Task AddAsync(PMVis entity);
        Task<PagedResult<PMVis>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PMVis> GetByIdAsync(long pmVisId);
        Task<bool> UpdateAsync(PMVis entity);
        Task<bool> PatchStatusAsync(long pmVisId, string status, long usersBy, long businessId);
    }
}
