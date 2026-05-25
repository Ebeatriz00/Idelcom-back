using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClientsActivityRepository
    {
        Task<long> AddActivityAsync(ClientsActivity entity);
        Task<PagedResult<ClientsActivity>> GetActivitiesAllAsync(long businessId, long clientsId, int page, int pageSize);
        Task<bool> DeleteActivityAsync(long businessId, long clientsActivityId);
        Task<bool> UpdateActivityStatusAsync(long businessId, long clientsActivityId, long activityStateId, long usersBy);
    }
}
