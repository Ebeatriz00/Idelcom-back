using Application.DTOs.Operations.OperationsWorkOrderStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderStatus
{
    public class GetSelectOperationsWorkOrderStatus(IOperationsWorkOrderStatusRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<OperationsWorkOrderStatusSelectDto>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<OperationsWorkOrderStatusSelectDto>>(result);
        }
    }

}
