using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBusinessLineRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(BusinessLine entity);
        Task<PagedResult<BusinessLine>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<BusinessLine> GetByIdAsync(long businessLineId);
        Task<bool> UpdateAsync(BusinessLine businessLine);
        Task<bool> PatchStatusAsync(long businessLineId, string status, long UsersBy, long businessId);
    }
}
