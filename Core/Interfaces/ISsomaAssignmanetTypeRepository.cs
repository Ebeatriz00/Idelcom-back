using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISsomaAssignmanetTypeRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? ssomaAssignTypeId = null);
        Task AddAsync(SsomaAssignmanetType entity);
        Task<PagedResult<SsomaAssignmanetType>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<SsomaAssignmanetType> GetByIdAsync(long ssomaAssignTypeId);
        Task<bool> UpdateAsync(SsomaAssignmanetType entity);
        Task<bool> PatchStatusAsync(long ssomaAssignTypeId, string status, long usersBy, long businessId);
    }
}
