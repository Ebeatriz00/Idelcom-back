using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISubTasksRepository
    {
        Task AddAsync(SubTasks entity);
        Task<bool> UpdateAsync(SubTasks entity);
        Task<bool> DeleteAsync(long subTasksId, long usersBy);
        Task<SubTasks> GetByIdAsync(long subTasksId);
        Task<List<SubTasks>> GetAllByTaskAsync(long businessId, long? tasksId);
    }
}
