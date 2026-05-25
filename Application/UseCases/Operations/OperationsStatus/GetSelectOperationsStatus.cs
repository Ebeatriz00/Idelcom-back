using Application.DTOs.Operations.OperationsStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsStatus
{
    public class GetSelectOperationsStatus(IOperationsStatusRepository repository, IMapper mapper)
    {
        private readonly IOperationsStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<OperationsStatusSelectDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, page, pageSize, search);

            return _mapper.Map<PagedSelect<OperationsStatusSelectDto>>(entities);
        }
    }
}
