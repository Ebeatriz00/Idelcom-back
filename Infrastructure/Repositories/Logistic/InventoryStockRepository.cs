using Core.Entities.Logistic;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class InventoryStockRepository(IDapperHelper dapperHelper) : IInventoryStockRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<bool> ExistsAsync(long businessId, long warehouseId, long productsId, long? excludeId = null)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@BUSINESS_ID", businessId)
                    .WithInput("@WAREHOUSE_ID", warehouseId)
                    .WithInput("@PRODUCTS_ID", productsId)
                    .WithInput("@ID", excludeId);

                const string query = """
                    SELECT COUNT(*)
                    FROM dbo.INVENTORY_STOCK
                    WHERE BUSINESS_ID = @BUSINESS_ID
                      AND WAREHOUSE_ID = @WAREHOUSE_ID
                      AND PRODUCTS_ID = @PRODUCTS_ID
                      AND (@ID IS NULL OR INVENTORY_STOCK_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de stock de inventario.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(InventoryStock entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.ProductsId,
                    entity.WarehouseId,
                    entity.StockQuantity,
                    entity.AverageCost,
                    entity.LastCost,
                    entity.LastEnteryDate,
                    entity.LastOutputDate,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_INVENTORY_STOCK",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar stock de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<InventoryStock?> GetByIdAsync(long inventoryStockId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    InventoryStockId = inventoryStockId
                });

                return await _dapperHelper.QueryFirstOrDefaultAsync<InventoryStock>(
                    "SP_WS_INVENTORY_STOCK_BY_ID",
                    parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener stock de inventario por ID.", ex.Message);
            }
        }

        public async Task<InventoryStock?> GetByProductAsync(long businessId, long warehouseId, long productsId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    WarehouseId = warehouseId,
                    ProductsId = productsId
                });

                return await _dapperHelper.QueryFirstOrDefaultAsync<InventoryStock>(
                    "SP_WS_INVENTORY_STOCK_BY_PRODUCT",
                    parameters,
                    transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener stock de inventario por producto.", ex.Message);
            }
        }

        public async Task IncreaseAsync(long businessId, long warehouseId, long productsId, decimal quantity, decimal unitCost, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    WarehouseId = warehouseId,
                    ProductsId = productsId,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    UserId = userId,
                });

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INCREASE_INVENTORY_STOCK",
                    parameters,
                    transaction);
            }
            catch (SqlException ex) when (ex.Number is 50000 or 51000 or 51001)
            {
                throw new BusinessException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al incrementar stock de inventario.", ex.Message);
            }
        }

        public async Task DecreaseAsync(long businessId, long warehouseId, long productsId, decimal quantity, long userId, bool allowNegative, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    WarehouseId = warehouseId,
                    ProductsId = productsId,
                    Quantity = quantity,
                    UserId = userId,
                    AllowNegative = allowNegative,
                });

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DECREASE_INVENTORY_STOCK",
                    parameters,
                    transaction);
            }
            catch (SqlException ex) when (ex.Number is 50000 or 51000 or 51001 or 51002)
            {
                throw new BusinessException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al disminuir stock de inventario.", ex.Message);
            }
        }
    }
}
