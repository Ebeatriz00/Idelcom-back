using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IMovementStatusRepository
    {
        Task<PagedSelect<MovementStatusSelectItem>> GetSelectAsync(long businessId, int page, int pageSize, string? search);
        Task<MovementStatus?> GetByIdAsync(long movementStatusId, long businessId);
    }

}
