using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityStateRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, string? excludeId = null);
        Task AddAsync(ActivityState entity);
        Task<PagedResult<ActivityState>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<List<ActivityState>> GetSelectAsync(long businessId);
        Task<ActivityState> GetByIdAsync(long likeToken);
        Task<bool> UpdateAsync(ActivityState entity);
        Task<bool> PatchStatusAsync(string likeToken, string status, long UsersBy, long businessId);
    }
}
