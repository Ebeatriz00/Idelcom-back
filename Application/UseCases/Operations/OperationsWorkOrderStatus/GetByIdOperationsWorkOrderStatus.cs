using Application.DTOs.Operations.OperationsWorkOrderStatus;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderStatus
{
    public class GetByIdOperationsWorkOrderStatus(IOperationsWorkOrderStatusRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsWorkOrderStatusGetByIdDto?> ExecuteAsync(
            long workOrderStatusId,
            long businessId)
        {
            var result = await _repository.GetByIdAsync(workOrderStatusId, businessId);

            if (result == null)
                throw new NotFoundException("Operations Work Order Status", workOrderStatusId);

            return _mapper.Map<OperationsWorkOrderStatusGetByIdDto>(result);
        }
    }

}
