using Core.Commands.Logistic;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Core.Results.Logistic;
using Dapper;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class PurchaseReceiptRepository(IDapperHelper dapperHelper) : IPurchaseReceiptRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(PurchaseReceiptCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.PurchaseOrderId,
                    command.SuppliersId,
                    command.WarehouseId,
                    command.ReceiptTypeId,
                    command.ReceiptDate,
                    command.SupplierGuideNumber,
                    command.SupplierGuideDate,
                    command.Observation,
                    command.UserId
                })
                .WithTable("@Details", CreateDetailsTable(command.Details), "dbo.PurchaseReceiptDetailInputType")
                .WithOutputLong("@Id")
                .WithOutputLong("@WarehouseMovementId")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CREATE_PURCHASE_RECEIPT", parameters, transaction);
                return BuildBaseResponseId(parameters, "No se pudo registrar la recepcion de compra.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la recepcion de compra en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<PurchaseReceiptListItem>> ListAsync(PurchaseReceiptFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.WarehouseId,
                    filter.SuppliersId,
                    filter.PurchaseOrderId,
                    filter.ReceiptStatusId,
                    filter.ReceiptTypeId,
                    filter.DateFrom,
                    filter.DateTo,
                    filter.Search,
                    filter.Page,
                    filter.PageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<PurchaseReceiptListItem>(
                    "SP_WS_LIST_PURCHASE_RECEIPT",
                    parameters);

                return new PagedResult<PurchaseReceiptListItem>
                {
                    Items = items.ToList(),
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar recepciones de compra.", ex.Message);
            }
        }

        public async Task<PurchaseReceiptByIdProjection?> GetByIdAsync(long businessId, long purchaseReceiptId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseReceiptId = purchaseReceiptId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GET_PURCHASE_RECEIPT_BY_ID",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<PurchaseReceiptHeaderProjection>();
                        var details = (await grid.ReadAsync<PurchaseReceiptDetailProjection>()).ToList();

                        if (header is null)
                            return null;

                        return new PurchaseReceiptByIdProjection
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
                throw new DatabaseException("Error al obtener la recepcion de compra por ID.", ex.Message);
            }
        }

        public async Task<BaseResponse> VoidAsync(long businessId, long purchaseReceiptId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseReceiptId = purchaseReceiptId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_VOID_PURCHASE_RECEIPT", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo anular la recepcion de compra.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al anular la recepcion de compra en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> RegularizeWithPurchaseOrderAsync(PurchaseReceiptRegularizeCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.PurchaseReceiptId,
                    command.PurchaseOrderId,
                    command.UserId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_REGULARIZE_PURCHASE_RECEIPT_WITH_PO", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo regularizar la recepcion de compra.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al regularizar la recepcion de compra en base de datos.", ex.Message);
            }
        }

        private static BaseResponseId BuildBaseResponseId(DapperParams parameters, string fallbackMessage)
        {
            var response = new PurchaseReceiptCreateResult
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput"),
                Id = parameters.Get<long>("@Id"),
                WarehouseMovementId = parameters.Get<long?>("@WarehouseMovementId")
            };

            if (response.Status != 1)
                throw new BusinessException(response.Message ?? fallbackMessage);

            return response;
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

        private static DataTable CreateDetailsTable(IReadOnlyList<PurchaseReceiptDetailCommand> details)
        {
            var table = new DataTable();
            table.Columns.Add("PURCHASE_ORDER_DETAIL_ID", typeof(long));
            table.Columns.Add("PRODUCTS_ID", typeof(long));
            table.Columns.Add("UOM_ID", typeof(long));
            table.Columns.Add("ORDERED_QUANTITY", typeof(decimal));
            table.Columns.Add("RECEIVED_QUANTITY", typeof(decimal));
            table.Columns.Add("UNIT_COST", typeof(decimal));
            table.Columns.Add("OBSERVATION", typeof(string));

            foreach (var detail in details)
            {
                table.Rows.Add(
                    detail.PurchaseOrderDetailId.HasValue ? detail.PurchaseOrderDetailId.Value : (object)DBNull.Value,
                    detail.ProductsId,
                    detail.UomId.HasValue ? detail.UomId.Value : (object)DBNull.Value,
                    detail.OrderedQuantity,
                    detail.ReceivedQuantity,
                    detail.UnitCost,
                    detail.Observation ?? (object)DBNull.Value);
            }

            return table;
        }
    }
}
