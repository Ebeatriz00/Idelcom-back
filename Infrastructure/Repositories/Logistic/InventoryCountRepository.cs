using Core.Commands.Logistic;
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
    public class InventoryCountRepository(IDapperHelper dapperHelper) : IInventoryCountRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(InventoryCountCreateCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.WarehouseId,
                    command.CountDate,
                    command.Observation,
                    command.UserId
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CREATE_INVENTORY_COUNT", parameters, transaction);
                return BuildBaseResponseId(parameters, "No se pudo crear la toma de inventario.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al crear la toma de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> StartAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    InventoryCountId = inventoryCountId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_START_INVENTORY_COUNT", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo iniciar la toma de inventario.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al iniciar la toma de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateDetailsAsync(InventoryCountUpdateDetailsCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.InventoryCountId,
                    command.UserId
                })
                .WithTable("@Details", CreateUpdateDetailsTable(command.Details), "dbo.InventoryCountDetailUpdateType")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_INVENTORY_COUNT_DETAILS", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo actualizar los detalles de la toma de inventario.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar detalles de toma de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> CloseAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    InventoryCountId = inventoryCountId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CLOSE_INVENTORY_COUNT", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo cerrar la toma de inventario.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cerrar la toma de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> CancelAsync(long businessId, long inventoryCountId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    InventoryCountId = inventoryCountId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CANCEL_INVENTORY_COUNT", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo anular la toma de inventario.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al anular la toma de inventario en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> MarkAsAdjustedAsync(InventoryCountMarkAdjustedCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.InventoryCountId,
                    command.UserId
                })
                .WithTable("@Adjustments", CreateAdjustmentsTable(command.Adjustments), "dbo.InventoryCountDetailAdjustmentType")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_MARK_INVENTORY_COUNT_AS_ADJUSTED", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo marcar la toma de inventario como ajustada.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al marcar la toma de inventario como ajustada en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<InventoryCountListItem>> ListAsync(InventoryCountFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.WarehouseId,
                    filter.InventoryCountStatusId,
                    filter.DateFrom,
                    filter.DateTo,
                    filter.Search,
                    filter.Page,
                    filter.PageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<InventoryCountListItem>(
                    "SP_WS_LIST_INVENTORY_COUNT",
                    parameters);

                return new PagedResult<InventoryCountListItem>
                {
                    Items = items.ToList(),
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar tomas de inventario.", ex.Message);
            }
        }

        public async Task<InventoryCountByIdProjection?> GetByIdAsync(long businessId, long inventoryCountId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    InventoryCountId = inventoryCountId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GET_INVENTORY_COUNT_BY_ID",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<InventoryCountHeaderProjection>();
                        var details = (await grid.ReadAsync<InventoryCountDetailProjection>()).ToList();

                        if (header is null)
                            return null;

                        return new InventoryCountByIdProjection
                        {
                            Header = header,
                            Details = details
                        };
                    },
                    parameters,
                    transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la toma de inventario por ID.", ex.Message);
            }
        }

        private static BaseResponseId BuildBaseResponseId(DapperParams parameters, string fallbackMessage)
        {
            var status = parameters.Get<int>("@COutput");
            var message = parameters.Get<string>("@SOutput");
            var id = parameters.Get<long>("@Id");

            if (status != 1)
                throw new BusinessException(message ?? fallbackMessage);

            return new BaseResponseId { Status = status, Message = message, Id = id };
        }

        private static BaseResponse BuildBaseResponse(DapperParams parameters, string fallbackMessage)
        {
            var response = new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };

            if (response.Status != 1)
                throw new BusinessException(response.Message ?? fallbackMessage);

            return response;
        }

        private static DataTable CreateUpdateDetailsTable(IReadOnlyList<InventoryCountDetailUpdateCommand> details)
        {
            var table = new DataTable();
            table.Columns.Add("INVENTORY_COUNT_DETAIL_ID", typeof(long));
            table.Columns.Add("COUNTED_QUANTITY", typeof(decimal));
            table.Columns.Add("LOT_NUMBER", typeof(string));
            table.Columns.Add("SERIAL_NUMBER", typeof(string));
            table.Columns.Add("EXPIRATION_DATE", typeof(DateTime));
            table.Columns.Add("OBSERVATION", typeof(string));

            foreach (var d in details)
            {
                table.Rows.Add(
                    d.InventoryCountDetailId,
                    d.CountedQuantity,
                    d.LotNumber ?? (object)DBNull.Value,
                    d.SerialNumber ?? (object)DBNull.Value,
                    d.ExpirationDate.HasValue ? d.ExpirationDate.Value : (object)DBNull.Value,
                    d.Observation ?? (object)DBNull.Value);
            }

            return table;
        }

        private static DataTable CreateAdjustmentsTable(IReadOnlyList<InventoryCountDetailAdjustmentCommand> adjustments)
        {
            var table = new DataTable();
            table.Columns.Add("INVENTORY_COUNT_DETAIL_ID", typeof(long));
            table.Columns.Add("ADJUSTMENT_MOVEMENT_ID", typeof(long));

            foreach (var a in adjustments)
                table.Rows.Add(a.InventoryCountDetailId, a.AdjustmentMovementId);

            return table;
        }
    }
}
