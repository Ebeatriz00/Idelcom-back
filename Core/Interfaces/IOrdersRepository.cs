using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrdersRepository
    {
        Task<PagedResult<Orders>> GetAllAsync(long businessId, string? search, long? responsibleStaff, int page, int pageSize);
        Task<bool> AddSsomaAsync(IEnumerable<Orders> entities);
        Task<bool> ExistsSsomaAsync(long operationsId, long workerId);
        Task<bool> AddQualitySupervisorAsync(Orders entity);
        Task<bool> AddProjectManagerAsync(Orders entity);
    }
}
