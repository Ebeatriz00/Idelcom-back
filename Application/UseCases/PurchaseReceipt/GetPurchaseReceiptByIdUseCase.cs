using Application.DTOs.PurchaseReceipt;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Logistic;

namespace Application.UseCases.PurchaseReceipt
{
    public class GetPurchaseReceiptByIdUseCase(IPurchaseReceiptRepository repository, IMapper mapper)
    {
        private readonly IPurchaseReceiptRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PurchaseReceiptFullResponseDto> ExecuteAsync(long businessId, long purchaseReceiptId)
        {
            var result = await _repository.GetByIdAsync(businessId, purchaseReceiptId);
            if (result?.Header is null)
                throw new NotFoundException("Recepcion de compra", purchaseReceiptId);

            return _mapper.Map<PurchaseReceiptFullResponseDto>(result);
        }
    }
}
