using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityRepository
    {
        Task AddActivityOpporAsync(Activity entity);
        Task AddActivityProjectAsync(Activity entity);
        Task<bool> DeleteActivityOpporAsync(string linkToken, string OpporToken);
        Task<bool> DeleteActivityProjectAsync(string  linkToken, string ProjectToken);
        Task<bool> PatchActivityOpporChangeStateAsync(string linkToken, string status, long usersBy, long businessId);
        Task<bool> PatchActivityOpporPriorityStateAsync(string linkToken, string status, long usersBy, long businessId);

    }
}
