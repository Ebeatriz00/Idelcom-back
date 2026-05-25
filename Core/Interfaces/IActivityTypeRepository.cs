using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityTypeRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, string? excludeId = null);
        Task AddAsync(ActivityType entity);
        Task<PagedResult<ActivityType>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<List<ActivityType>> GetSelectAsync(long businessId);
        Task<ActivityType> GetByIdAsync(string likeToken);
        Task<bool> UpdateAsync(ActivityType entity);
        Task<bool> PatchStatusAsync(string likeToken, string status, long UsersBy, long businessId);
    }
}
