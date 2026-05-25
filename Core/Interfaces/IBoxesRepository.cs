using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBoxesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Boxes entity);
        Task<PagedResult<Boxes>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Boxes> GetByIdAsync(long boxesId);
        Task<bool> UpdateAsync(Boxes entity);
        Task<bool> PatchStatusAsync(long boxesId, string status, long usersBy, long businessId);
    }
}
