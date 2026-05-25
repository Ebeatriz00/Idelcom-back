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
    public class PurchaseOrderRepository(IDapperHelper dapperHelper) : IPurchaseOrderRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> RegisterAsync(PurchaseOrderCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = CreateHeaderParameters(command)
                    .WithTable("@Details", CreateDetailsTable(command.Details), "dbo.PurchaseOrderDetailInputType")
                    .WithOutputLong("@Id")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_REGISTER_PURCHASE_ORDER", parameters, transaction);

                return BuildBaseResponseId(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la orden de compra en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<PurchaseOrderListItem>> ListAsync(PurchaseOrderFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.SuppliersId,
                    filter.PurchaseOrderStatusId,
                    filter.DateFrom,
                    filter.DateTo,
                    filter.Search,
                    filter.PageNumber,
                    filter.PageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<PurchaseOrderListItem>(
                    "SP_WS_LIST_PURCHASE_ORDER",
                    parameters);

                return new PagedResult<PurchaseOrderListItem>
                {
                    Items = items.ToList(),
                    Page = filter.PageNumber,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar ordenes de compra.", ex.Message);
            }
        }

        public async Task<PurchaseOrderByIdProjection?> GetByIdAsync(long businessId, long purchaseOrderId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GET_PURCHASE_ORDER_BY_ID",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<PurchaseOrderHeaderProjection>();
                        var details = (await grid.ReadAsync<PurchaseOrderDetailProjection>()).ToList();
                        var invoices = (await grid.ReadAsync<PurchaseOrderInvoiceProjection>()).ToList();

                        if (header is null)
                            return null;

                        return new PurchaseOrderByIdProjection
                        {
                            Header = header,
                            Details = details,
                            Invoices = invoices
                        };
                    },
                    parameters,
                    transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la orden de compra por ID.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(PurchaseOrderCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = CreateHeaderParameters(command)
                    .WithInput("@PurchaseOrderId", command.PurchaseOrderId)
                    .WithTable("@Details", CreateDetailsTable(command.Details), "dbo.PurchaseOrderDetailInputType")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_PURCHASE_ORDER", parameters, transaction);

                return BuildBaseResponse(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la orden de compra en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> ApproveAsync(long businessId, long purchaseOrderId, long approvedBy, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId,
                    ApprovedBy = approvedBy,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_APPROVE_PURCHASE_ORDER", parameters, transaction);
                return BuildBaseResponse(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al aprobar la orden de compra en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponse> SendForApprovalAsync(long businessId, long purchaseOrderId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_SEND_PURCHASE_ORDER_FOR_APPROVAL", parameters, transaction);
                return BuildBaseResponse(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al enviar la orden de compra a aprobacion.", ex.Message);
            }
        }

        public async Task<BaseResponse> CancelAsync(long businessId, long purchaseOrderId, long cancelledBy, string? reason, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId,
                    CancelledBy = cancelledBy,
                    Reason = reason,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CANCEL_PURCHASE_ORDER", parameters, transaction);
                return BuildBaseResponse(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al anular la orden de compra en base de datos.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AttachInvoiceAsync(PurchaseOrderAttachInvoiceCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.PurchaseOrderId,
                    command.SupplierInvoiceId,
                    command.RegularizationReason,
                    command.UserId
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_ATTACH_PURCHASE_ORDER_INVOICE", parameters, transaction);
                return BuildBaseResponseId(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al asociar la factura proveedor a la orden de compra.", ex.Message);
            }
        }

        public async Task<BaseResponseId> CreateFromInvoiceAsync(PurchaseOrderCreateFromInvoiceCommand command, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.SupplierInvoiceId,
                    command.WarehouseId,
                    command.Observation,
                    command.UserId
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CREATE_PURCHASE_ORDER_FROM_INVOICE", parameters, transaction);
                return BuildBaseResponseId(parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al crear la orden de compra desde factura proveedor.", ex.Message);
            }
        }

        public async Task<PurchaseOrderInvoiceProjection?> GetPurchaseOrderInvoiceByIdAsync(long businessId, long purchaseOrderInvoiceId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderInvoiceId = purchaseOrderInvoiceId
                });

                return await _dapperHelper.QueryFirstOrDefaultAsync<PurchaseOrderInvoiceProjection>(
                    "SP_WS_GET_PURCHASE_ORDER_INVOICE_BY_ID",
                    parameters,
                    transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la relacion entre orden de compra y factura proveedor.", ex.Message);
            }
        }

        public async Task<PurchaseOrderInvoiceProjection?> GetPurchaseOrderInvoiceAsync(long businessId, long purchaseOrderId, long supplierInvoiceId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId,
                    SupplierInvoiceId = supplierInvoiceId
                });

                return await _dapperHelper.QueryFirstOrDefaultAsync<PurchaseOrderInvoiceProjection>(
                    "SP_WS_GET_PURCHASE_ORDER_INVOICE_BY_PAIR",
                    parameters,
                    transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la relacion entre orden de compra y factura proveedor.", ex.Message);
            }
        }

        public async Task<PurchaseOrderPdfProjection?> GetPdfDataAsync(long businessId, long purchaseOrderId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PurchaseOrderId = purchaseOrderId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GET_PURCHASE_ORDER_PDF",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<PurchaseOrderPdfHeaderProjection>();
                        var details = (await grid.ReadAsync<PurchaseOrderPdfDetailProjection>()).ToList();

                        if (header is null)
                            return null;

                        return new PurchaseOrderPdfProjection
                        {
                            Header = header,
                            Details = details
                        };
                    },
                    parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener los datos PDF de la orden de compra.", ex.Message);
            }
        }

        private static DapperParams CreateHeaderParameters(PurchaseOrderCommand command)
        {
            return DapperParams.From(new
            {
                command.BusinessId,
                command.SuppliersId,
                command.PurchaseOrderDate,
                command.CurrencyId,
                command.ExchangeRate,
                command.PmConditionId,
                command.ExpectedDeliveryDate,
                command.WarehouseId,
                command.SupplierQuotationReferenceNumber,
                command.References,
                command.Observation,
                command.UserId
            });
        }

        private static BaseResponseId BuildBaseResponseId(DapperParams parameters)
        {
            var response = new BaseResponseId
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput"),
                Id = parameters.Get<long>("@Id")
            };

            if (response.Status != 1)
                throw new BusinessException(response.Message ?? "No se pudo procesar la orden de compra.");

            return response;
        }

        private static BaseResponse BuildBaseResponse(DapperParams parameters)
        {
            var response = new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };

            if (response.Status != 1)
                throw new BusinessException(response.Message ?? "No se pudo procesar la orden de compra.");

            return response;
        }

        private static DataTable CreateDetailsTable(IReadOnlyList<PurchaseOrderDetailCommand> details)
        {
            var table = new DataTable();
            table.Columns.Add("PURCHASE_ORDER_DETAIL_ID", typeof(long));
            table.Columns.Add("PRODUCTS_ID", typeof(long));
            table.Columns.Add("UOM_ID", typeof(long));
            table.Columns.Add("QUANTITY", typeof(decimal));
            table.Columns.Add("UNIT_PRICE", typeof(decimal));
            table.Columns.Add("DISCOUNT_PERCENT", typeof(decimal));
            table.Columns.Add("TAXES_ID", typeof(long));
            table.Columns.Add("PRICE_INCLUDES_TAX", typeof(bool));
            table.Columns.Add("OBSERVATION", typeof(string));
            table.Columns.Add("IS_ACTIVE", typeof(bool));

            foreach (var detail in details)
            {
                table.Rows.Add(
                    detail.PurchaseOrderDetailId.HasValue ? detail.PurchaseOrderDetailId.Value : (object)DBNull.Value,
                    detail.ProductsId,
                    detail.UomId.HasValue ? detail.UomId.Value : (object)DBNull.Value,
                    detail.Quantity,
                    detail.UnitPrice,
                    detail.DiscountPercent,
                    detail.TaxesId.HasValue ? detail.TaxesId.Value : (object)DBNull.Value,
                    detail.PriceIncludesTax,
                    detail.Observation ?? (object)DBNull.Value,
                    detail.IsActive);
            }

            return table;
        }
    }
}
