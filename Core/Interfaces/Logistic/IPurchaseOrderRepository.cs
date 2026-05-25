using Core.Commands.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Projections.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IPurchaseOrderRepository
    {
        Task<BaseResponseId> RegisterAsync(PurchaseOrderCommand command, IDbTransaction transaction);
        Task<PagedResult<PurchaseOrderListItem>> ListAsync(PurchaseOrderFilter filter);
        Task<PurchaseOrderByIdProjection?> GetByIdAsync(long businessId, long purchaseOrderId, IDbTransaction? transaction = null);
        Task<BaseResponse> UpdateAsync(PurchaseOrderCommand command, IDbTransaction transaction);
        Task<BaseResponse> ApproveAsync(long businessId, long purchaseOrderId, long approvedBy, long userId, IDbTransaction transaction);
        Task<BaseResponse> CancelAsync(long businessId, long purchaseOrderId, long cancelledBy, string? reason, long userId, IDbTransaction transaction);
        Task<BaseResponseId> AttachInvoiceAsync(PurchaseOrderAttachInvoiceCommand command, IDbTransaction transaction);
        Task<BaseResponseId> CreateFromInvoiceAsync(PurchaseOrderCreateFromInvoiceCommand command, IDbTransaction transaction);
        Task<PurchaseOrderInvoiceProjection?> GetPurchaseOrderInvoiceByIdAsync(long businessId, long purchaseOrderInvoiceId, IDbTransaction? transaction = null);
        Task<PurchaseOrderInvoiceProjection?> GetPurchaseOrderInvoiceAsync(long businessId, long purchaseOrderId, long supplierInvoiceId, IDbTransaction? transaction = null);
        Task<PurchaseOrderPdfProjection?> GetPdfDataAsync(long businessId, long purchaseOrderId);
        Task<BaseResponse> SendForApprovalAsync(long businessId, long purchaseOrderId, long userId, IDbTransaction transaction);
    }
}
