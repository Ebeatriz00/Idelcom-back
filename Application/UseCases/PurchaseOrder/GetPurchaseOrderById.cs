using Application.DTOs.PurchaseOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Logistic;

namespace Application.UseCases.PurchaseOrder
{
    public class GetPurchaseOrderById(IPurchaseOrderRepository repository, IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PurchaseOrderGetByIdResponse> ExecuteAsync(long businessId, long purchaseOrderId)
        {
            var result = await _repository.GetByIdAsync(businessId, purchaseOrderId);
            if (result?.Header is null)
                throw new NotFoundException("Orden de compra", purchaseOrderId);

            var response = _mapper.Map<PurchaseOrderGetByIdResponse>(result.Header);
            response.Details = _mapper.Map<List<PurchaseOrderDetailResponse>>(result.Details);
            response.Invoices = _mapper.Map<List<PurchaseOrderInvoiceResponse>>(result.Invoices);
            return response;
        }
    }
}
