using Application.DTOs.Operations.Operations;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.Operations
{
    public class GetByIdOperations(IOperationsRepository repository, IMapper mapper)
    {
        private readonly IOperationsRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsResponseDto?> ExecuteAsync(long operationsId)
        {
            var projection = await _repository.GetByIdAsync(operationsId);
            return _mapper.Map<OperationsResponseDto?>(projection);
        }
    }
}
