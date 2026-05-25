using Application.DTOs.PurchaseOrder;
using AutoMapper;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using FluentValidation;

namespace Application.UseCases.PurchaseOrder
{
    public class ListPurchaseOrder(
        IPurchaseOrderRepository repository,
        IValidator<PurchaseOrderListFilterDto> validator,
        IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IValidator<PurchaseOrderListFilterDto> _validator = validator;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<PurchaseOrderListResponse>> ExecuteAsync(PurchaseOrderListFilterDto filter, long businessId)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
            await PurchaseOrderValidation.ValidateAsync(_validator, filter);

            var result = await _repository.ListAsync(new PurchaseOrderFilter
            {
                BusinessId = businessId,
                SuppliersId = NormalizeId(filter.SuppliersId),
                PurchaseOrderStatusId = NormalizeId(filter.PurchaseOrderStatusId),
                DateFrom = filter.DateFrom,
                DateTo = filter.DateTo,
                Search = filter.Search,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            });

            return new PagedResult<PurchaseOrderListResponse>
            {
                Items = _mapper.Map<List<PurchaseOrderListResponse>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }

        private static long? NormalizeId(long? value)
        {
            return value.HasValue && value.Value > 0 ? value.Value : null;
        }
    }
}
