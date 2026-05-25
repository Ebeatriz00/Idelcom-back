using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovClasRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(MovClas entity);
        Task<PagedResult<MovClas>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovClas> GetByIdAsync(long movClasId);
        Task<bool> UpdateAsync(MovClas entity);
        Task<bool> PatchStatusAsync(long movClasId, string status, long usersBy, long businessId);
    }
}
