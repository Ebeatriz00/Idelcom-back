using Application.DTOs.Operations.OperationsWorkOrderProgress;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderProgress
{
    public class GetOperationsWorkOrderProgressList(
        IOperationsWorkOrderProgressRepository repository,
        IMapper mapper)
    {
        private readonly IOperationsWorkOrderProgressRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsWorkOrderProgressResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? activityId, string? date, long? operationsId)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize, activityId, date, operationsId);
            return _mapper.Map<PagedResult<OperationsWorkOrderProgressResponseDto>>(pagedEntity);
        }
    }
}