using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovementTypesRepository
    {
        Task<bool> ExistsByDescriptionAsync(string description, long businessId, long? excludeId = null);
        Task<bool> ExistsByCodeAsync(string code, long businessId, long? excludeId = null);
        Task AddAsync(MovementTypes entity);
        Task<PagedResult<MovementTypes>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovementTypes> GetByIdAsync(long movementTypesId);
        Task<MovementTypes?> GetByCodeAsync(string code, long businessId);
        Task<bool> UpdateAsync(MovementTypes entity);
        Task<bool> PatchStatusAsync(long movementTypesId, string status, long usersBy, long businessId);
    }
}
