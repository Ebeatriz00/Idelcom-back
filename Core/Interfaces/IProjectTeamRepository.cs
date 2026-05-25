using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProjectTeamRepository
    {
        Task<bool> ExistsAsync(long businessId, long projectId, long workerId, long? excludeId = null);
        Task AddAsync(IEnumerable<ProjectTeam> entity);
        Task<PagedResult<ProjectTeam>> GetAllAsync(long businessId, long? projectId, string? search, int page, int pageSize);
        Task<bool> DeleteAsync(long businessId, long projectTeamId);

    }
}
