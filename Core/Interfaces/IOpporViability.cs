using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOpporViabilityRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, string? excludeId = null);
        Task<PagedResult<OpporViability>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<bool> PatchStatusAsync(string linkToken, string status, long usersBy, long businessId);
        Task<(bool Success, string Message)> ProcessDecisionAsync(long opporId, long businessId, long usersBy, bool isApproved, string? rejectionReason);
    }
}
