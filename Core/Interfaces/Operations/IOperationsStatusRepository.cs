using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IOperationsStatusRepository
    {
        Task<PagedSelect<OperationsStatusSelectItem>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
        Task<OperationsStatus?> GetByIdAsync(long workOrderStatusId, long businessId);
    }
}
