using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISsomaMovementTypeRepository
    {
        Task AddAsync(SsomaMovementType entity);
        Task<PagedResult<SsomaMovementType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<SsomaMovementType> GetByIdAsync(long ssomaMovementTypeId);
        Task<bool> UpdateAsync(SsomaMovementType entity);
        Task<bool> PatchStatusAsync(long ssomaMovementTypeId, string status, long usersBy, long businessId);
    }
}
