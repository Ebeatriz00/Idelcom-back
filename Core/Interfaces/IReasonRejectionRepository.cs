using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReasonRejectionRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(ReasonRejection entity);
        Task<PagedResult<ReasonRejection>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ReasonRejection> GetByIdAsync(long reasonRejectionId);
        Task<bool> UpdateAsync(ReasonRejection entity);
        Task<bool> PatchStatusAsync(long reasonRejectionId, string status, long usersBy, long businessId);
    }
}
