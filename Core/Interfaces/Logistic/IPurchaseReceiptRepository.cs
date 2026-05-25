using Core.Commands.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Projections.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IPurchaseReceiptRepository
    {
        Task<BaseResponseId> CreateAsync(PurchaseReceiptCommand command, IDbTransaction transaction);
        Task<PagedResult<PurchaseReceiptListItem>> ListAsync(PurchaseReceiptFilter filter);
        Task<PurchaseReceiptByIdProjection?> GetByIdAsync(long businessId, long purchaseReceiptId, IDbTransaction? transaction = null);
        Task<BaseResponse> VoidAsync(long businessId, long purchaseReceiptId, long userId, IDbTransaction transaction);
        Task<BaseResponse> RegularizeWithPurchaseOrderAsync(PurchaseReceiptRegularizeCommand command, IDbTransaction transaction);
    }
}
