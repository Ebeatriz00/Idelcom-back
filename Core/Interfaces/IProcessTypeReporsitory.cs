using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProcessTypeReporsitory
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(ProcessType entity);
        Task<PagedResult<ProcessType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ProcessType> GetByIdAsync(long processTypeId);
        Task<bool> UpdateAsync(ProcessType processType);
        Task<bool> PatchStatusAsync(long processTypeId, string status, long UsersBy, long businessId);
    }
}
