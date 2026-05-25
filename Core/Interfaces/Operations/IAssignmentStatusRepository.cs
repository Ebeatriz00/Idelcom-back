using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IAssignmentStatusRepository
    {
        Task<PagedSelect<AssignmentStatusSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
        Task<AssignmentStatus?> GetByIdAsync(long id, long businessId);
    }

}
