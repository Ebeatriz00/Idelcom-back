using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobTitleRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(JobTitle entity);
        Task<PagedResult<JobTitle>> GetAllAsync(long businessId,string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetJobTitleForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<JobTitle> GetByIdAsync(long jobTitleId);
        Task<bool> UpdateAsync(JobTitle jobTitle);
        Task<bool> PatchStatusAsync(long jobTitleId, string status, long updatedBy, long businessId);
    }
}
