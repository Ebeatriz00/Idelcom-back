using Application.DTOs.Operations.OperationsStatus;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsStatus
{
    public class GetByIdOperationsStatus(IOperationsStatusRepository repository, IMapper mapper)
    {
        private readonly IOperationsStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsStatusGeByIdDto?> ExecuteAsync(
            long operationsStatusId,
            long businessId)
        {
            var result = await _repository.GetByIdAsync(operationsStatusId, businessId);

            if (result == null)
                throw new NotFoundException("Operations Status", operationsStatusId);

            return _mapper.Map<OperationsStatusGeByIdDto>(result);
        }
    }

}
