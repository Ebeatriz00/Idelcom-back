using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;

namespace Application.UseCases.PurchaseOrderDetailStatus
{
    public class GetSelectPurchaseOrderDetailStatus
    {
        private readonly IPurchaseOrderDetailStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectPurchaseOrderDetailStatus(IPurchaseOrderDetailStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
