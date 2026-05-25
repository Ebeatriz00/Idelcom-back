using Application.DTOs.PurchaseReceipt;
using AutoMapper;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using FluentValidation;

namespace Application.UseCases.PurchaseReceipt
{
    public class ListPurchaseReceiptUseCase(
        IPurchaseReceiptRepository repository,
        IValidator<PurchaseReceiptListFilterDto> validator,
        IMapper mapper)
    {
        private readonly IPurchaseReceiptRepository _repository = repository;
        private readonly IValidator<PurchaseReceiptListFilterDto> _validator = validator;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<PurchaseReceiptResponseDto>> ExecuteAsync(PurchaseReceiptListFilterDto filter, long businessId)
        {
            filter.BusinessId = businessId;
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
            await PurchaseReceiptValidation.ValidateAsync(_validator, filter);

            var result = await _repository.ListAsync(new PurchaseReceiptFilter
            {
                BusinessId = businessId,
                WarehouseId = NormalizeId(filter.WarehouseId),
                SuppliersId = NormalizeId(filter.SuppliersId),
                PurchaseOrderId = NormalizeId(filter.PurchaseOrderId),
                ReceiptStatusId = NormalizeId(filter.ReceiptStatusId),
                ReceiptTypeId = NormalizeId(filter.ReceiptTypeId),
                DateFrom = filter.DateFrom,
                DateTo = filter.DateTo,
                Search = filter.Search,
                Page = filter.Page,
                PageSize = filter.PageSize
            });

            return new PagedResult<PurchaseReceiptResponseDto>
            {
                Items = _mapper.Map<List<PurchaseReceiptResponseDto>>(result.Items),
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
