using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using System.Threading.Tasks;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class GetSelectOperationsWorkOrderActivity(IOperationsWorkOrderActivityRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<OperationsWorkOrderActivitySelectItem?>> ExecuteAsync(
            long businessId,
            long operationsId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, operationsId, page, pageSize, search);
            return _mapper.Map<PagedSelect<OperationsWorkOrderActivitySelectItem?>>(result);
        }
    }
}