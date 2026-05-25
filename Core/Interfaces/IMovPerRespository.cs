using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovPerRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(MovPer entity);
        Task<PagedResult<MovPer>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovPer> GetByIdAsync(long movPerId);
        Task<bool> UpdateAsync(MovPer entity);
        Task<bool> PatchStatusAsync(long movPerId, string status, long usersBy, long businessId);
    }
}
