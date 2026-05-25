using Application.DTOs.Operations.OperationsWorkOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class GetByIdOperationsWorkOrder(IOperationsWorkOrderRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsWorkOrderByIdDto> ExecuteAsync(long workOrderId, long businessId)
        {
            var result = await _repository.GetByIdAsync(workOrderId, businessId);

            if (result == null)
                throw new NotFoundException("Operations Work Order", workOrderId);

            return _mapper.Map<OperationsWorkOrderByIdDto>(result);
        }
    }
}
