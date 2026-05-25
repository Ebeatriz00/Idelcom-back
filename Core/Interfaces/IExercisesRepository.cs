using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IExercisesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(Exercises entity);
        Task<PagedResult<Exercises>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Exercises> GetByIdAsync(long exercisesId);
        Task<bool> UpdateAsync(Exercises exercisesId);
        Task<bool> PatchStatusAsync(long exercisesId, string status, long usersBy, long businessId);
        Task<bool> PatchBlockStatusAsync(long exercisesId, bool indBlock, long usersBy, long businessId);
    }
}
