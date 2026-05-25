using Application.DTOs.Operations.OperationsWorkOrderActivity;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class GetAllOperationsWorkOrderActivity(IOperationsWorkOrderActivityRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsWorkOrderActivityResponseDto>> ExecuteAsync(
            long workOrderId,
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetAllAsync(workOrderId, businessId, page, pageSize, search);
            return _mapper.Map<PagedResult<OperationsWorkOrderActivityResponseDto>>(result);
        }
    }
}
