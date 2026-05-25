using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovOperRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(MovOper entity);
        Task<PagedResult<MovOper>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovOper> GetByIdAsync(long movOperId);
        Task<bool> UpdateAsync(MovOper entity);
        Task<bool> PatchStatusAsync(long movOperId, string status, long usersBy, long businessId);
    }
}
