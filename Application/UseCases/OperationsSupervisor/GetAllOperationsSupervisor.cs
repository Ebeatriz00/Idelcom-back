using Application.DTOs.OperationsSupervisor;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.OperationsSupervisor;
using SharedKernel;

namespace Application.UseCases.OperationsSupervisor
{
    public class GetAllOperationsSupervisor( IOperationsSupervisorRepository repository,IMapper mapper)
    {
        private readonly IOperationsSupervisorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsSupervisorResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<OperationsSupervisorResponseDto>>(pagedEntity);
        }
    }
}
