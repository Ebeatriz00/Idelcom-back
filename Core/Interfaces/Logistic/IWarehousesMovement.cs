using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Projections.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IWarehousesMovement
    {
        Task<BaseResponseId> AddAsync(
            WarehousesMovement entity,
            IReadOnlyCollection<WarehouseMovementDetail> details,
            IDbTransaction transaction);
        Task<BaseResponseId> AddDetailAsync(WarehouseMovementDetail entity, IDbTransaction transaction);
        Task<PagedResult<WarehouseMovementListItem>> ListAsync(WarehouseMovementFilter filter);
        Task<WarehouseMovementByIdProjection?> GetByIdAsync(long businessId, long warehouseMovementId);
        Task<IReadOnlyList<InventoryStockAvailableProjection>> GetAvailableStockAsync(InventoryStockAvailableFilter filter);
    }
}
