using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Dapper;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class WarehousesMovementRepository(IDapperHelper dapperHelper) : IWarehousesMovement
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> AddAsync(
            WarehousesMovement entity,
            IReadOnlyCollection<WarehouseMovementDetail> details,
            IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.MovementTypeId,
                    entity.WarehouseId,
                    entity.WarehouseDestinationId,
                    entity.SuppliersId,
                    entity.ClientsId,
                    entity.Series,
                    entity.NumberDocument,
                    entity.ReferenceDocument,
                    entity.MovementDate,
                    entity.Observation,
                    entity.TaxesId,
                    entity.CreateUser
                })
                .WithTable("@Details", CreateDetailsTable(details), "dbo.WarehouseMovementDetailInputType")
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500)
                .WithOutputLong("@TaxesIdOutput")
                .WithOutputDecimal("@SubTotalOutput")
                .WithOutputDecimal("@IgvPercentOutput")
                .WithOutputDecimal("@IgvOutput")
                .WithOutputDecimal("@TotalOutput");

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_WAREHOUSES_MOVEMENT",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                entity.TaxesId = parameters.Get<long?>("@TaxesIdOutput");
                entity.SubTotal = parameters.Get<decimal>("@SubTotalOutput");
                entity.IgvPercent = parameters.Get<decimal>("@IgvPercentOutput");
                entity.Igv = parameters.Get<decimal>("@IgvOutput");
                entity.Total = parameters.Get<decimal>("@TotalOutput");

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el movimiento de almacen en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddDetailAsync(WarehouseMovementDetail entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.WarehouseMovementId,
                    entity.BusinessId,
                    entity.ProductsId,
                    entity.Quantity,
                    entity.UnitCost,
                    entity.TotalCost,
                    entity.LotNumber,
                    entity.SerialNumber,
                    entity.ExpirationDate,
                    entity.Observation,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_WAREHOUSES_MOVEMENT_DETAIL",
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
                throw new DatabaseException("Error al registrar el detalle del movimiento de almacen en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<WarehouseMovementListItem>> ListAsync(WarehouseMovementFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.PageNumber,
                    filter.PageSize,
                    filter.Search,
                    filter.MovementTypeId,
                    filter.MovOperId,
                    filter.WarehouseId,
                    filter.DateFrom,
                    filter.DateTo
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<WarehouseMovementListItem>(
                    "SP_WS_LIST_WAREHOUSE_MOVEMENT",
                    parameters);

                return new PagedResult<WarehouseMovementListItem>
                {
                    Items = items.ToList(),
                    Page = filter.PageNumber,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar movimientos de almacen.", ex.Message);
            }
        }

        public async Task<WarehouseMovementByIdProjection?> GetByIdAsync(long businessId, long warehouseMovementId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    WarehouseMovementId = warehouseMovementId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GET_WAREHOUSE_MOVEMENT_BY_ID",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<WarehouseMovementHeaderProjection>();
                        var details = (await grid.ReadAsync<WarehouseMovementDetailProjection>()).ToList();

                        if (header is null)
                            return null;

                        return new WarehouseMovementByIdProjection
                        {
                            Header = header,
                            Details = details
                        };
                    },
                    parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener movimiento de almacen por ID.", ex.Message);
            }
        }

        public async Task<IReadOnlyList<InventoryStockAvailableProjection>> GetAvailableStockAsync(InventoryStockAvailableFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.WarehouseId,
                    filter.ProductsId,
                    filter.Search
                });

                var items = await _dapperHelper.QueryAsync<InventoryStockAvailableProjection>(
                    "SP_WS_GET_INVENTORY_STOCK_AVAILABLE",
                    parameters);

                return items.ToList();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al consultar stock disponible.", ex.Message);
            }
        }

        private static DataTable CreateDetailsTable(IReadOnlyCollection<WarehouseMovementDetail> details)
        {
            var table = new DataTable();
            table.Columns.Add("ProductsId", typeof(long));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("UnitCost", typeof(decimal));

            foreach (var detail in details)
            {
                table.Rows.Add(detail.ProductsId, detail.Quantity, detail.UnitCost);
            }

            return table;
        }
    }
}
