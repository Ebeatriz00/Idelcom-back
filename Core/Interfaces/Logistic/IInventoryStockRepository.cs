using Core.Entities.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IInventoryStockRepository
    {
        Task<bool> ExistsAsync(long businessId, long warehouseId, long productsId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(InventoryStock entity, IDbTransaction transaction);
        Task<InventoryStock?> GetByIdAsync(long inventoryStockId);
        Task<InventoryStock?> GetByProductAsync(long businessId, long warehouseId, long productsId, IDbTransaction? transaction = null);
        Task IncreaseAsync(long businessId, long warehouseId, long productsId, decimal quantity, decimal unitCost, long userId, IDbTransaction transaction);
        Task DecreaseAsync(long businessId, long warehouseId, long productsId, decimal quantity, long userId, bool allowNegative, IDbTransaction transaction);
    }
}
