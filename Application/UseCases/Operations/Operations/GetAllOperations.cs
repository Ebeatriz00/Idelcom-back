using Application.DTOs.Operations.Operations;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.Operations
{
    public class GetAllOperations(IOperationsRepository repository, IMapper mapper)
    {
        private readonly IOperationsRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsResponseDto>> ExecuteAsync(long businessId, int page, int pageSize)
        {
            var entity = await _repository.GetAllAsync(businessId, page, pageSize);

            return _mapper.Map<PagedResult<OperationsResponseDto>>(entity);
        }
    }
}
