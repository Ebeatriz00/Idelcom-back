using Application.DTOs.Operations.OperationsWorkOrder;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class GetAllOperationsWorkOrder(IOperationsWorkOrderRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsWorkOrderListDto>> ExecuteAsync(long businessId, int page, int pageSize, string? search, long? operationsId)
        {
            var result = await _repository.GetAllAsync(businessId, page, pageSize, search, operationsId);

            return _mapper.Map<PagedResult<OperationsWorkOrderListDto>>(result);
        }
    }
}
