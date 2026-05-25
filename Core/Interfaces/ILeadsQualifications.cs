using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILeadsQualificationsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(LeadsQualifications entity);
        Task<PagedResult<LeadsQualifications>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<LeadsQualifications> GetByIdAsync(long leadsQualificationsId);
        Task<bool> UpdateAsync(LeadsQualifications entity);
        Task<bool> PatchStatusAsync(long leadsQualificationsId, string status, long usersBy, long businessId);
    }
}
