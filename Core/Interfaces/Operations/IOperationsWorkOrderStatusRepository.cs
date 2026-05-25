using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderStatusRepository
    {
        Task<PagedSelect<OperationsWorkOrderStatusSelectItem>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
        Task<OperationWorkOrderStatus?> GetByIdAsync(long workOrderDtatusId, long businessId);
    }
}
