using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class GetSelectOperationsWorkOrder(IOperationsWorkOrderRepository repository)
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;

        public async Task<PagedSelect<OperationsWorkOrderSelectItem?>> ExecuteAsync(long businessId, long operationsId, int page, int pageSize, string? search)
        {
            return await _repository.GetForSelectAsync(businessId, operationsId, page, pageSize, search);
        }
    }
}
