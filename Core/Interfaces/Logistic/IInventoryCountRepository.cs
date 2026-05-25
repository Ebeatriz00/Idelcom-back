using Core.Commands.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Projections.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IInventoryCountRepository
    {
        Task<BaseResponseId> CreateAsync(InventoryCountCreateCommand command, IDbTransaction transaction);
        Task<BaseResponse> StartAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction);
        Task<BaseResponse> UpdateDetailsAsync(InventoryCountUpdateDetailsCommand command, IDbTransaction transaction);
        Task<BaseResponse> CloseAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction);
        Task<BaseResponse> CancelAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction);
        Task<BaseResponse> MarkAsAdjustedAsync(InventoryCountMarkAdjustedCommand command, IDbTransaction transaction);
        Task<PagedResult<InventoryCountListItem>> ListAsync(InventoryCountFilter filter);
        Task<InventoryCountByIdProjection?> GetByIdAsync(long businessId, long inventoryCountId, IDbTransaction? transaction = null);
    }
}
