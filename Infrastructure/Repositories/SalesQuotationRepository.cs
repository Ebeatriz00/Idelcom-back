using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using SharedKernel.Constants;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SalesQuotationRepository : ISalesQuotationRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public SalesQuotationRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> AddAsync(SalesQuotation entity, CancellationToken ct = default)
        {
            await using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync(ct);

            await using var tx = await cn.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                long quotationId;
                long quotationVerId;
                int versionNo;

                // 1) Cabecera
                using (var cmd = new SqlCommand("SP_WS_SALES_QT_HDR_CREATE", cn, (SqlTransaction)tx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 15;
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@OPPOR_ID", (object?)entity.OpporId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPPOR_NUMBER", (object?)entity.OpporNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPPOR_NAME", (object?)entity.OpporName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLIENTS_ID", (object?)entity.ClientsId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLIENTS_NAME", (object?)entity.ClientsName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CURRENCY_ID", (object?)entity.CurrencyId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CURRENCY_NAME", (object?)entity.CurrencyName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EXCHANGE_RATE", (object?)entity.ExchangeRate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PAYMENT_CONDITION_ID", (object?)entity.PaymentConditionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PAYMENT_CONDITION_NAME", (object?)entity.PaymentConditionName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OFFER_VALIDITY", (object?)entity.OfferValidity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@START_DATE", (object?)entity.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FINISH_DATE", (object?)entity.FinishDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SUB_TOTAL", entity.SubTotal ?? 0);
                    cmd.Parameters.AddWithValue("@DISCOUNT_AMOUNT", entity.DiscountAmount ?? 0);
                    cmd.Parameters.AddWithValue("@TAX_AMOUNT", entity.TaxAmount ?? 0);
                    cmd.Parameters.AddWithValue("@TOTAL_AMOUNT", entity.Total ?? 0);
                    cmd.Parameters.AddWithValue("@VVC_TOTAL", entity.VvcTotal ?? 0);
                    cmd.Parameters.AddWithValue("@SALES_TOTAL", entity.SalesTotal ?? 0);
                    cmd.Parameters.AddWithValue("@COST_TOTAL", entity.CostTotal ?? 0);
                    cmd.Parameters.AddWithValue("@UTILITY_TOTAL", entity.UtilityTotal ?? 0);
                    cmd.Parameters.AddWithValue("@MARGIN_PERCENT", entity.MarginPercent ?? 0);
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count == 0)
                            throw new DatabaseException("No se devolvió información de la cotización.");

                        quotationId = Convert.ToInt64(dt.Rows[0]["QUOTATION_ID"]);
                        quotationVerId = Convert.ToInt64(dt.Rows[0]["QUOTATION_VER_ID"]);
                        versionNo = Convert.ToInt32(dt.Rows[0]["VERSION_NO"]);
                    }
                }

                await InsertQuotationBodyAsync(cn, (SqlTransaction)tx, quotationVerId, entity, ct);

                await tx.CommitAsync(ct);
                return new GlobalResponse { Status = 1, Message = "Cotización creada correctamente" };
            }
            catch (SqlException ex)
            {
                try { await tx.RollbackAsync(ct); } catch { }
                Console.WriteLine(ex.ToString());
                throw new DatabaseException("Error al crear la cotización.", ex.Message);
            }
            catch (Exception ex)
            {
                try { await tx.RollbackAsync(ct); } catch { }
                throw;
            }
        }

        public async Task<GlobalResponse> AddVerAsync(SalesQuotation entity, CancellationToken ct = default)
        {
            await using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync(ct);

            await using var tx = await cn.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                long quotationId;
                long quotationVerId;
                int versionNo;

                // 1) Cabecera
                using (var cmd = new SqlCommand("SP_WS_SALES_QT_HDR_VER_CREATE", cn, (SqlTransaction)tx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 15;
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@OPPOR_ID", (object?)entity.OpporId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPPOR_NUMBER", (object?)entity.OpporNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLIENTS_ID", (object?)entity.ClientsId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLIENTS_NAME", (object?)entity.ClientsName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SUB_TOTAL", entity.SubTotal ?? 0);
                    cmd.Parameters.AddWithValue("@DISCOUNT_AMOUNT", entity.DiscountAmount ?? 0);
                    cmd.Parameters.AddWithValue("@TAX_AMOUNT", entity.TaxAmount ?? 0);
                    cmd.Parameters.AddWithValue("@TOTAL_AMOUNT", entity.Total ?? 0);
                    cmd.Parameters.AddWithValue("@VVC_TOTAL", entity.VvcTotal ?? 0);
                    cmd.Parameters.AddWithValue("@SALES_TOTAL", entity.SalesTotal ?? 0);
                    cmd.Parameters.AddWithValue("@COST_TOTAL", entity.CostTotal ?? 0);
                    cmd.Parameters.AddWithValue("@UTILITY_TOTAL", entity.UtilityTotal ?? 0);
                    cmd.Parameters.AddWithValue("@MARGIN_PERCENT", entity.MarginPercent ?? 0);
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count == 0)
                            throw new DatabaseException("No se devolvió información de la cotización.");

                        quotationId = Convert.ToInt64(dt.Rows[0]["QUOTATION_ID"]);
                        quotationVerId = Convert.ToInt64(dt.Rows[0]["QUOTATION_VER_ID"]);
                        versionNo = Convert.ToInt32(dt.Rows[0]["VERSION_NO"]);
                    }
                }

                await InsertQuotationBodyAsync(cn, (SqlTransaction)tx, quotationVerId, entity, ct);

                await tx.CommitAsync(ct);
                return new GlobalResponse { Status = 1, Message = "Cotización creada correctamente" };
            }
            catch (SqlException ex)
            {
                try { await tx.RollbackAsync(ct); } catch { }
                Console.WriteLine(ex.ToString());
                throw new DatabaseException("Error al crear la cotización.", ex.Message);
            }
            catch (Exception ex)
            {
                try { await tx.RollbackAsync(ct); } catch { }
                throw;
            }
        }

        private async Task InsertQuotationBodyAsync(
            SqlConnection cn,
            SqlTransaction tx,
            long quotationVerId,
            SalesQuotation entity,
            CancellationToken ct)
        {
            // 2) Detalle
            var lineIdMap = new Dictionary<int, long>();

            // PRIMERO: Insertar todas las líneas PARENT
            var parentLines = entity.Lines
                .Where(x => x.LineNo.HasValue && (x.LineType == QuotationLineType.Parent || x.LevelNo == 1))
                .OrderBy(x => x.LineNo!.Value)
                .ToList();

            foreach (var line in parentLines)
            {
                var newId = await InsertLineAsync(
                    cn,
                    tx,
                    entity.BusinessId,
                    quotationVerId,
                    line,
                    null,
                    entity.UsersBy,
                    ct
                );

                lineIdMap[line.LineNo!.Value] = newId;
            }

            // SEGUNDO: Insertar todas las líneas no-PARENT
            var nonParentLines = entity.Lines
                .Where(x => x.LineNo.HasValue && x.LineType != QuotationLineType.Parent && x.LevelNo != 1)
                .OrderBy(x => x.LineNo!.Value)
                .ToList();

            foreach (var line in nonParentLines)
            {
                int lineNo = line.LineNo!.Value;
                long? parentId = null;

                var possibleParents = lineIdMap.Keys
                    .Where(k => k < lineNo)
                    .OrderByDescending(k => k)
                    .ToList();

                if (possibleParents.Any())
                {
                    var parentLineNo = possibleParents.First();
                    parentId = lineIdMap[parentLineNo];
                }

                var newId = await InsertLineAsync(
                    cn,
                    tx,
                    entity.BusinessId,
                    quotationVerId,
                    line,
                    parentId,
                    entity.UsersBy,
                    ct
                );

                lineIdMap[lineNo] = newId;
            }

            // 3) Márgenes
            if (entity.Margins != null && entity.Margins.Any())
            {
                foreach (var margin in entity.Margins)
                {
                    using var cmd = new SqlCommand("SP_WS_SALES_QT_MARGIN_VER_CREATE", cn, tx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 15;
                    cmd.Parameters.AddWithValue("@QUOTATION_VER_ID", quotationVerId);
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@MARGIN_TYPE_ID", (object?)margin.MarginTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MARGIN_TYPE_NAME", (object?)margin.MarginTypeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MARGIN_RATE", margin.MarginRate ?? 0);
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);
                    await cmd.ExecuteNonQueryAsync(ct);
                }
            }

            // 4) Servicios
            if (entity.ServChecks != null && entity.ServChecks.Any())
            {
                var sc = entity.ServChecks.First();
                using var cmd = new SqlCommand("SP_WS_SALES_QT_SERV_CHECK_VER_CREATE", cn, tx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;
                cmd.Parameters.Add("@QUOTATION_VER_ID", SqlDbType.BigInt).Value = quotationVerId;
                cmd.Parameters.Add("@QUOTATION_AMOUNT", SqlDbType.Decimal).Value = (object?)sc.QuotationAmount ?? DBNull.Value;
                cmd.Parameters.Add("@SCHEDULE_AMOUNT", SqlDbType.Decimal).Value = (object?)sc.ScheduleAmount ?? DBNull.Value;
                cmd.Parameters.Add("@DIFFERENCE_AMOUNT", SqlDbType.Decimal).Value = (object?)sc.DifferenceAmount ?? DBNull.Value;
                cmd.Parameters.Add("@CREATE_USER", SqlDbType.BigInt).Value = entity.UsersBy;
                await cmd.ExecuteNonQueryAsync(ct);
            }

            // 5) PLANES DE PAGO
            if (entity.LinePlans is { Count: > 0 })
            {
                if (entity.LinePlans is { Count: > 0 } && (entity.Lines == null || entity.Lines.Count == 0))
                    throw new Exception("Hay LinePlans pero no hay Lines. Revisa que el parser esté seteando dto.Lines.");

                await InsertLinePlansAsync(cn, tx, lineIdMap, entity.LinePlans, entity.UsersBy, ct);
            }

            // 6) EGRESOS
            if (entity.Egresses is { Count: > 0 })
            {
                await InsertEgressesAsync(cn, tx, quotationVerId, entity.Egresses, entity.UsersBy, ct);
            }
        }

        private static async Task<long> InsertLineAsync(
            SqlConnection cn,
            SqlTransaction tx,
            long businessId,
            long quotationVerId,
            SalesQuotationLin line,
            long? parentQuotationVerLinId,
            long createUser,
            CancellationToken ct)
        {
            using var cmd = new SqlCommand("SP_WS_SALES_QT_VER_LIN_CREATE", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 15;

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@QUOTATION_VER_ID", quotationVerId);
            cmd.Parameters.AddWithValue("@LINE_NO", line.LineNo ?? 0);
            cmd.Parameters.AddWithValue("@DISPLAY_NO", (object?)line.DisplayNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LINE_TYPE", (object?)line.LineType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LEVEL_NO", (object?)line.LevelNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IS_ROLLUP", (object?)line.IsRollUp ?? false);
            cmd.Parameters.AddWithValue("@PARENT_QUOTATION_VER_LIN_ID", (object?)parentQuotationVerLinId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ITEM_ID", (object?)line.ItemId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PRODUCTS_ID", (object?)line.ProductsId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DESCRIPTION", (object?)line.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PRODUCTS_TYPE_ID", (object?)line.ProductsTypeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PRODUCTS_TYPE_NAME", (object?)line.ProductsTypeName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UOM_ID", (object?)line.UomId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UOM_NAME", (object?)line.UomName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BRAND_ID", (object?)line.BrandsId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BRAND", (object?)line.Brands ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MODEL", (object?)line.Model ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QTY", (object?)line.Qty ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UNIT_PRICE", (object?)line.UnitPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TOTAL_PRICE", (object?)line.TotalPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DISCOUNT_AMOUNT", (object?)line.DiscountAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TAX_AMOUNT", (object?)line.TaxAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LINE_TOTAL", (object?)line.LineAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IS_PRESALES", (object?)line.IsPresales ?? false);
            cmd.Parameters.AddWithValue("@IS_BOLD", (object?)line.IsBold ?? false);
            cmd.Parameters.AddWithValue("@UNIT_COST", (object?)line.UnitCost ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LINE_COST_TOTAL", (object?)line.LineCostTotal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UNIT_SALE_PRICE", (object?)line.UnitSalePrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LINE_SALE_TOTAL", (object?)line.LineSaleTotal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MARGIN_PERCENT_LINE", (object?)line.MargenPorcentLine ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PRESALES_ASSIGNED_ID", (object?)line.PresalesAssignedId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PRESALES_ASSIGNED_TO", (object?)line.PresalesAssignedTo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SYSTEM_ID", (object?)line.SystemId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SYSTEM_NAME", (object?)line.SystemName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SUPPLIERS_ID", (object?)line.SuppliersId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SUPPLIERS_NAME", (object?)line.SuppliersName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DELIVERY_DAYS", (object?)line.DeliveryDays ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PM_CONDITION_ID", (object?)line.PmConditionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PM_CONDITION_NAME", (object?)line.PmConditionName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PM_CONDITION_DAY", (object?)line.PmConditionDay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ORDER_MONTH_NO", (object?)line.OrderMonthNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CREATE_USER", createUser);

            var obj = await cmd.ExecuteScalarAsync(ct);
            if (obj == null || obj == DBNull.Value)
                throw new DatabaseException("No se devolvió QUOTATION_VER_LIN_ID al insertar detalle.");

            return Convert.ToInt64(obj);
        }

        private static async Task InsertLinePlansAsync(
            SqlConnection cn,
            SqlTransaction tx,
            Dictionary<int, long> lineIdMap,
            List<SalesQuotationLinePlan> plans,
            long createUser,
            CancellationToken ct)
        {
            foreach (var plan in plans)
            {
                if (!lineIdMap.TryGetValue(plan.TempLineNo, out var quotationVerLinId))
                {
                    if (lineIdMap.Count == 0)
                        throw new Exception("lineIdMap está vacío. No se insertó ningún detalle (Lines).");

                    var min = lineIdMap.Keys.Min();
                    var max = lineIdMap.Keys.Max();
                    throw new Exception($"No existe TempLineNo={plan.TempLineNo}. Rango {min}..{max}");
                }

                // 1) HDR
                long linePlanId;
                using (var cmdHdr = new SqlCommand("SP_WS_SALES_QT_LINE_PLAN_VER_HDR_CREATE", cn, tx))
                {
                    cmdHdr.CommandType = CommandType.StoredProcedure;
                    cmdHdr.CommandTimeout = 15;
                    cmdHdr.Parameters.AddWithValue("@QUOTATION_VER_LIN_ID", quotationVerLinId);

                    var obj = await cmdHdr.ExecuteScalarAsync(ct);
                    if (obj == null || obj == DBNull.Value)
                        throw new DatabaseException("No devolvió LINE_PLAN_ID al crear HDR de plan.");

                    linePlanId = Convert.ToInt64(obj);
                }

                // 2) LIN
                if (plan.Lines == null || plan.Lines.Count == 0) continue;

                foreach (var lin in plan.Lines.OrderBy(x => x.SeqNo))
                {
                    var hasPercent = lin.PaymentPercent.HasValue && lin.PaymentPercent.Value > 0m;
                    var hasAmount = lin.PaymentAmount.HasValue && lin.PaymentAmount.Value > 0m;

                    if (!hasPercent && !hasAmount)
                        continue;

                    using var cmdLin = new SqlCommand("SP_WS_SALES_QT_LINE_PLAN_VER_LIN_CREATE", cn, tx);
                    cmdLin.CommandType = CommandType.StoredProcedure;
                    cmdLin.CommandTimeout = 15;

                    cmdLin.Parameters.AddWithValue("@LINE_PLAN_ID", linePlanId);
                    cmdLin.Parameters.AddWithValue("@SEQ_NO", lin.SeqNo);
                    cmdLin.Parameters.AddWithValue("@MONTH_NO", lin.MonthNo);
                    cmdLin.Parameters.AddWithValue("@PAYMENT_NO", lin.PaymentNo);
                    cmdLin.Parameters.AddWithValue("@PAYMENT_PERCENT", (object?)lin.PaymentPercent ?? DBNull.Value);
                    cmdLin.Parameters.AddWithValue("@PAYMENT_AMOUNT", (object?)lin.PaymentAmount ?? DBNull.Value);

                    await cmdLin.ExecuteNonQueryAsync(ct);
                }
            }
        }

        private static async Task InsertEgressesAsync(
            SqlConnection cn,
            SqlTransaction tx,
            long quotationVerId,
            List<SalesQuotationEgress> egresses,
            long createUser,
            CancellationToken ct)
        {
            foreach (var hdr in egresses.OrderBy(x => x.MonthNo))
            {
                // 1) HDR
                long egressVerId;
                using (var cmdHdr = new SqlCommand("SP_WS_SALES_QT_EGRESS_VER_HDR_CREATE", cn, tx))
                {
                    cmdHdr.CommandType = CommandType.StoredProcedure;
                    cmdHdr.CommandTimeout = 15;
                    cmdHdr.Parameters.AddWithValue("@QUOTATION_VER_ID", quotationVerId);
                    cmdHdr.Parameters.AddWithValue("@MONTH_NO", hdr.MonthNo);
                    cmdHdr.Parameters.AddWithValue("@AMOUNT", hdr.Amount);
                    cmdHdr.Parameters.AddWithValue("@CREATE_USER", createUser);

                    var obj = await cmdHdr.ExecuteScalarAsync(ct);
                    if (obj == null || obj == DBNull.Value)
                        throw new DatabaseException("No devolvió EGRESS_VER_ID al crear HDR de egresos.");

                    egressVerId = Convert.ToInt64(obj);
                }

                // 2) LIN
                if (hdr.Lines == null || hdr.Lines.Count == 0) continue;

                foreach (var lin in hdr.Lines)
                {

                    using var cmdLin = new SqlCommand("SP_WS_SALES_QT_EGRESS_VER_LIN_CREATE", cn, tx);
                    cmdLin.CommandType = CommandType.StoredProcedure;
                    cmdLin.CommandTimeout = 15;

                    cmdLin.Parameters.AddWithValue("@EGRESS_VER_ID", egressVerId);
                    cmdLin.Parameters.AddWithValue("@LINE_NO", lin.LineNo);
                    cmdLin.Parameters.AddWithValue("@MONTH_NO", lin.MonthNo);
                    cmdLin.Parameters.AddWithValue("@AMOUNT", lin.Amount);
                    cmdLin.Parameters.AddWithValue("@CREATE_USER", createUser);

                    await cmdLin.ExecuteNonQueryAsync(ct);
                }
            }
        }

        //--- Lista de cotizaciones
        public async Task<PagedResult<SalesQuotation>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersId, string? verDesc, long? workerId)
        {
            try
            {

                var list = new List<SalesQuotation>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                ;
                using var cmd = new SqlCommand("SP_WS_SALES_QT_LIST", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)search ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VER_DESC", (object?)verDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)workerId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var quotId = reader.GetInt64(0);

                    list.Add(new SalesQuotation
                    {
                        QuotationId = _linkTokenService.Issue("quotation", quotId, TimeSpan.FromHours(1)),
                        QuotationNo = reader.IsDBNull(1) ? null : reader.GetString(1),
                        OpporDesc = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ClientsName = reader.IsDBNull(3) ? null : reader.GetString(3),
                        WorkerName = reader.IsDBNull(4) ? null : reader.GetString(4),
                        CurrencySymbol = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Total= reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                        QuotationStatus = reader.IsDBNull(7) ? null : reader.GetString(7),
                        VersionStatus = reader.IsDBNull(8) ? null : reader.GetString(8),
                        CreatedDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                        Status = reader.IsDBNull(10) ? null : reader.GetString(10),
                        VersionNo = reader.IsDBNull(11) ? null : reader.GetString(11),
                        QuotationColor = reader.IsDBNull(12) ? null : reader.GetString(12),
                        VersionColor = reader.IsDBNull(13) ? null : reader.GetString(13)

                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<SalesQuotation>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de cliente paginada.", ex.Message);
            }
        }

        //--- Lista de versiones de cotizaciones
        public async Task<PagedResult<SalesQuotation>> GetAllVerAsync(string quotationId, long businessId, string? search, int page, int pageSize, string? verDesc, long? workerId, long? workerResponsibles)
        {
            try
            {

                var list = new List<SalesQuotation>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                ;
                using var cmd = new SqlCommand("SP_WS_SALES_QT_VER_LIST", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@QUOTATION_ID", quotationId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)search ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@VER_DESC", (object?)verDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)workerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WORKER_RESPONSIBLE", (object?)workerResponsibles ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var quotVerId = reader.GetInt64(0);

                    list.Add(new SalesQuotation
                    {
                        QuotationVerId = _linkTokenService.Issue("quotation-ver", quotVerId, TimeSpan.FromHours(1)),
                        QuotationNo = reader.IsDBNull(1) ? null : reader.GetString(1),
                        ClientsName = reader.IsDBNull(2) ? null : reader.GetString(2),
                        WorkerResponsible = reader.IsDBNull(3) ? null : reader.GetString(3),
                        CurrencySymbol = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Total = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        VersionStatus = reader.IsDBNull(6) ? null : reader.GetString(6),
                        VersionColor = reader.IsDBNull(7) ? null : reader.GetString(7),
                        VersionNo = reader.IsDBNull(8) ? null : reader.GetString(8),
                        CreatedDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                        

                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<SalesQuotation>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de cliente paginada.", ex.Message);
            }
        }

        //--- Detalle de cotización
        public async Task<SalesQuotation?> GetDetailAsync(
            long quotationVerId,
            long businessId,
            string? versionNo,
            CancellationToken ct = default
            )
        {
            await using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync(ct);

            try
            {
                var qvId = quotationVerId;

                await using var cmd = new SqlCommand("SP_WS_SALES_QT_VER_DETAIL_ALL", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add(new SqlParameter("@BUSINESS_ID", SqlDbType.BigInt) { Value = businessId });
                cmd.Parameters.Add(new SqlParameter("@QUOTATION_VER_ID", SqlDbType.BigInt) { Value = qvId });
                cmd.Parameters.Add(new SqlParameter("@VERSION_NO", SqlDbType.VarChar, 100)
                {
                    Value = (object?)versionNo ?? DBNull.Value
                });

                await using var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default, ct);

                // ===== Helpers ultra rápidos =====
                static string? NStr(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetString(i);
                static long? N64(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetInt64(i);
                static int? N32(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetInt32(i);
                static decimal? NDec(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetDecimal(i);
                static DateTime? NDt(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetDateTime(i);
                static bool? NBool(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetBoolean(i);
                static bool Bool0(SqlDataReader rd, int i) => !rd.IsDBNull(i) && rd.GetBoolean(i);

                // =========================
                // RS1: HEADER
                // =========================
                if (!await r.ReadAsync(ct))
                    return null;

                // Ordinals cacheados (1 vez)
                var hOpporId = r.GetOrdinal("OPPOR_ID");
                var hClientsId = r.GetOrdinal("CLIENTS_ID");
                var hQuotationStatusId = r.GetOrdinal("QUOTATION_STATUS_ID");
                var hCurrencyId = r.GetOrdinal("CURRENCY_ID");
                var hExchangeRate = r.GetOrdinal("EXCHANGE_RATE");
                var hPmConditionId = r.GetOrdinal("PM_CONDITION_ID"); 
                var hOfferValidity = r.GetOrdinal("OFFER_VALIDITY");
                var hStartDate = r.GetOrdinal("START_DATE");
                var hFinishDate = r.GetOrdinal("FINISH_DATE");
                var hCurrentVersionNo = r.GetOrdinal("CURRENT_VERSION_NO");
                var hVersionNo = r.GetOrdinal("VERSION_NO");          
                var hVerStatusId = r.GetOrdinal("VER_STATUS_ID");     

                var hSubTotal = r.GetOrdinal("SUB_TOTAL");
                var hDiscountAmount = r.GetOrdinal("DISCOUNT_AMOUNT");
                var hTaxAmount = r.GetOrdinal("TAX_AMOUNT");
                var hTotalAmount = r.GetOrdinal("TOTAL_AMOUNT");
                var hUtilityTotal = r.GetOrdinal("UTILITY_TOTAL");
                var hMarginPercent = r.GetOrdinal("MARGIN_PERCENT");
                var hSalesTotal = r.GetOrdinal("SALES_TOTAL");
                var hCostTotal = r.GetOrdinal("COST_TOTAL");

                var hOpporDesc = r.GetOrdinal("OPPOR_DESC");
                var hClientsName = r.GetOrdinal("CLIENTS_NAME");
                var hCurrencyName = r.GetOrdinal("CURRENCY_NAME");
                var hPmConditionName = r.GetOrdinal("PM_CONDITION_NAME_HDR");
                var hOpporNumber = r.GetOrdinal("OPPOR_NUM");

                var qv = new SalesQuotation
                {
                    QuotationVerId = quotationVerId.ToString(),
                    BusinessId = businessId,
                    VersionNo = NStr(r, hVersionNo),

                    OpporId = N64(r, hOpporId),
                    ClientsId = N64(r, hClientsId),
                    QuotationStatusId = N64(r, hQuotationStatusId),

                    CurrencyId = N64(r, hCurrencyId),
                    ExchangeRate = NDec(r, hExchangeRate),

                    PaymentConditionId = N64(r, hPmConditionId),
                    OfferValidity = N32(r, hOfferValidity),
                    StartDate = NDt(r, hStartDate),
                    FinishDate = NDt(r, hFinishDate),

                    CurrentVersionNo = N32(r, hCurrentVersionNo),
                    VersionStatusId = N64(r, hVerStatusId),

                    SubTotal = NDec(r, hSubTotal),
                    DiscountAmount = NDec(r, hDiscountAmount),
                    TaxAmount = NDec(r, hTaxAmount),
                    Total = NDec(r, hTotalAmount),
                    UtilityTotal = NDec(r, hUtilityTotal),
                    MarginPercent = NDec(r, hMarginPercent),
                    SalesTotal = NDec(r, hSalesTotal),
                    CostTotal = NDec(r, hCostTotal),

                    OpporName = NStr(r, hOpporDesc),
                    ClientsName = NStr(r, hClientsName),
                    CurrencyName = NStr(r, hCurrencyName),
                    PaymentConditionName = NStr(r, hPmConditionName),
                    OpporNumber = NStr(r, hOpporNumber),

                    Lines = new List<SalesQuotationLin>(256),
                    Margins = new List<SalesQuotationMargin>(16),
                    ServChecks = new List<SalesQuotationServCheck>(16),
                    LinePlans = new List<SalesQuotationLinePlan>(64),
                    Egresses = new List<SalesQuotationEgress>(24),
                };

                // =========================
                // RS2: LINES
                // =========================
                await r.NextResultAsync(ct);

                if (r.HasRows)
                {
                    var lId = r.GetOrdinal("QUOTATION_VER_LIN_ID");
                    var lLineNo = r.GetOrdinal("LINE_NO");
                    var lLineType = r.GetOrdinal("LINE_TYPE");
                    var lParentId = r.GetOrdinal("PARENT_QUOTATION_VER_LIN_ID");
                    var lDisplayNo = r.GetOrdinal("DISPLAY_NO");
                    var lLevelNo = r.GetOrdinal("LEVEL_NO");
                    var lIsRollup = r.GetOrdinal("IS_ROLLUP");
                    var lItemId = r.GetOrdinal("ITEM_ID");
                    var lProductsId = r.GetOrdinal("PRODUCTS_ID");
                    var lDesc = r.GetOrdinal("DESCRIPTION");
                    var lProductsTypeId = r.GetOrdinal("PRODUCTS_TYPE_ID");
                    var lUomId = r.GetOrdinal("UOM_ID");
                    var lBrandId = r.GetOrdinal("BRAND_ID");
                    var lModel = r.GetOrdinal("MODEL");
                    var lQty = r.GetOrdinal("QTY");
                    var lUnitPrice = r.GetOrdinal("UNIT_PRICE");
                    var lTotalPrice = r.GetOrdinal("TOTAL_PRICE");
                    var lDiscount = r.GetOrdinal("DISCOUNT_AMOUNT");
                    var lTax = r.GetOrdinal("TAX_AMOUNT");
                    var lLineTotal = r.GetOrdinal("LINE_TOTAL");
                    var lIsPresales = r.GetOrdinal("IS_PRESALES");
                    var lIsBold = r.GetOrdinal("IS_BOLD");
                    var lUnitCost = r.GetOrdinal("UNIT_COST");
                    var lLineCostTotal = r.GetOrdinal("LINE_COST_TOTAL");
                    var lUnitSalePrice = r.GetOrdinal("UNIT_SALE_PRICE");
                    var lLineSaleTotal = r.GetOrdinal("LINE_SALE_TOTAL");
                    var lMarginLine = r.GetOrdinal("MARGIN_PERCENT_LINE");
                    var lPresalesAssignedId = r.GetOrdinal("PRESALES_ASSIGNED_ID");
                    var lPresalesAssignedTo = r.GetOrdinal("PRESALES_ASSIGNED_TO");
                    var lSystemId = r.GetOrdinal("SYSTEM_ID");
                    var lSuppliersId = r.GetOrdinal("SUPPLIERS_ID");
                    var lDeliveryDays = r.GetOrdinal("DELIVERY_DAYS");
                    var lPmConditionId = r.GetOrdinal("PM_CONDITION_ID");
                    var lOrderMonthNo = r.GetOrdinal("ORDER_MONTH_NO");

                    while (await r.ReadAsync(ct))
                    {
                        qv.Lines!.Add(new SalesQuotationLin
                        {
                            QuotationVerId = qvId,
                            QuotationVerLinId = N64(r, lId),
                            LineNo = N32(r, lLineNo),
                            LineType = NStr(r, lLineType),
                            ParentQuotationVerLinId = N64(r, lParentId),
                            DisplayNo = NStr(r, lDisplayNo),
                            LevelNo = N32(r, lLevelNo),
                            IsRollUp = NBool(r, lIsRollup),

                            ItemId = N64(r, lItemId),
                            ProductsId = N64(r, lProductsId),
                            Description = NStr(r, lDesc),

                            ProductsTypeId = N64(r, lProductsTypeId),
                            UomId = N64(r, lUomId),
                            BrandsId = N64(r, lBrandId),
                            Model = NStr(r, lModel),

                            Qty = NDec(r, lQty),
                            UnitPrice = NDec(r, lUnitPrice),
                            TotalPrice = NDec(r, lTotalPrice),

                            DiscountAmount = NDec(r, lDiscount),
                            TaxAmount = NDec(r, lTax),
                            LineAmount = NDec(r, lLineTotal),

                            IsPresales = Bool0(r, lIsPresales),
                            IsBold = Bool0(r, lIsBold),

                            UnitCost = NDec(r, lUnitCost),
                            LineCostTotal = NDec(r, lLineCostTotal),
                            UnitSalePrice = NDec(r, lUnitSalePrice),
                            LineSaleTotal = NDec(r, lLineSaleTotal),

                            MargenPorcentLine = NDec(r, lMarginLine),

                            PresalesAssignedId = N64(r, lPresalesAssignedId),
                            PresalesAssignedTo = NStr(r, lPresalesAssignedTo),

                            SystemId = N64(r, lSystemId),
                            SuppliersId = N64(r, lSuppliersId),

                            DeliveryDays = N32(r, lDeliveryDays),
                            PmConditionId = N64(r, lPmConditionId),
                            OrderMonthNo = N32(r, lOrderMonthNo),
                        });
                    }
                }

                // =========================
                // RS3: MARGINS
                // =========================
                await r.NextResultAsync(ct);

                if (r.HasRows)
                {
                    var mId = r.GetOrdinal("QUOTATION_MARGIN_ID");
                    var mTypeId = r.GetOrdinal("MARGIN_TYPE_ID");
                    var mRate = r.GetOrdinal("MARGIN_RATE");

                    var mMarginTypeName = r.GetOrdinal("MARGIN_TYPE_NAME");
                    while (await r.ReadAsync(ct))
                    {
                        qv.Margins!.Add(new SalesQuotationMargin
                        {
                            QuotationVerId = qvId,
                            QuotationMarginVerId = r.IsDBNull(mId) ? 0 : r.GetInt64(mId),
                            MarginTypeId = N64(r, mTypeId),
                            MarginRate = NDec(r, mRate),

                            MarginTypeName = NStr(r, mMarginTypeName),
                        });
                    }
                }

                // =========================
                // RS4: SERVICE CHECKS
                // =========================
                await r.NextResultAsync(ct);

                if (r.HasRows)
                {
                    var sId = r.GetOrdinal("SERVICE_CHECK_ID");
                    var sQ = r.GetOrdinal("QUOTATION_AMOUNT");
                    var sSch = r.GetOrdinal("SCHEDULE_AMOUNT");
                    var sDiff = r.GetOrdinal("DIFFERENCE_AMOUNT");

                    while (await r.ReadAsync(ct))
                    {
                        qv.ServChecks!.Add(new SalesQuotationServCheck
                        {
                            QuotationVerId = qvId,
                            ServiceCheckId = r.GetInt64(sId),
                            QuotationAmount = NDec(r, sQ),
                            ScheduleAmount = NDec(r, sSch),
                            DifferenceAmount = NDec(r, sDiff),
                        });
                    }
                }

                // =========================
                // RS5: PLAN HDR
                // =========================
                await r.NextResultAsync(ct);

                var plansById = new Dictionary<long, SalesQuotationLinePlan>(64);

                if (r.HasRows)
                {
                    var pId = r.GetOrdinal("LINE_PLAN_ID");
                    // var pLinId = r.GetOrdinal("QUOTATION_VER_LIN_ID"); // tu entidad no lo guarda

                    while (await r.ReadAsync(ct))
                    {
                        var planId = r.GetInt64(pId);

                        var plan = new SalesQuotationLinePlan
                        {
                            LinePlanId = planId,
                            Lines = new List<SalesQuotationLinePlanLin>(8)
                        };

                        qv.LinePlans!.Add(plan);
                        plansById[planId] = plan;
                    }
                }

                // =========================
                // RS6: PLAN LINES
                // =========================
                await r.NextResultAsync(ct);

                if (r.HasRows)
                {
                    var plId = r.GetOrdinal("LINE_PLAN_ID");
                    var plSeq = r.GetOrdinal("SEQ_NO");
                    var plMonth = r.GetOrdinal("MONTH_NO");
                    var plPayMonth = r.GetOrdinal("PAYMENT_NO");
                    var plPct = r.GetOrdinal("PAYMENT_PERCENT");
                    var plAmt = r.GetOrdinal("PAYMENT_AMOUNT");

                    while (await r.ReadAsync(ct))
                    {
                        var planId = r.GetInt64(plId);
                        if (!plansById.TryGetValue(planId, out var plan))
                            continue;

                        plan.Lines.Add(new SalesQuotationLinePlanLin
                        {
                            SeqNo = r.GetInt32(plSeq),
                            PaymentPercent = NDec(r, plPct),
                            PaymentAmount = NDec(r, plAmt),
                            MonthNo = r.GetInt32(plMonth),
                            PaymentNo = r.GetInt32(plPayMonth),
                        });
                    }
                }

                // =========================
                // RS7: EGRESOS HDR
                // =========================
                await r.NextResultAsync(ct);

                // Join interno por EGRESS_VER_ID (tu entidad no lo tiene)
                var egressById = new Dictionary<long, SalesQuotationEgress>(24);

                if (r.HasRows)
                {
                    var eId = r.GetOrdinal("EGRESS_VER_ID");
                    var eMonth = r.GetOrdinal("MONTH_NO");
                    var eAmt = r.GetOrdinal("AMOUNT");

                    while (await r.ReadAsync(ct))
                    {
                        var id = r.GetInt64(eId);

                        var eg = new SalesQuotationEgress
                        {
                            MonthNo = r.GetInt32(eMonth),
                            Amount = r.GetDecimal(eAmt),
                            Lines = new List<SalesQuotationEgressLin>(8)
                        };

                        qv.Egresses!.Add(eg);
                        egressById[id] = eg;
                    }
                }

                // =========================
                // RS8: EGRESOS LINES
                // =========================
                await r.NextResultAsync(ct);

                if (r.HasRows)
                {
                    var elEid = r.GetOrdinal("EGRESS_VER_ID");
                    var elLineNo = r.GetOrdinal("LINE_NO");
                    var elMonth = r.GetOrdinal("MONTH_NO");
                    var elAmt = r.GetOrdinal("AMOUNT");

                    while (await r.ReadAsync(ct))
                    {
                        var id = r.GetInt64(elEid);
                        if (!egressById.TryGetValue(id, out var eg))
                            continue;

                        eg.Lines.Add(new SalesQuotationEgressLin
                        {
                            LineNo = r.GetInt32(elLineNo),
                            MonthNo = r.GetInt32(elMonth),
                            Amount = r.GetDecimal(elAmt)
                        });
                    }
                }

                return qv;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener detalle de cotización.", ex.Message);
            }
        }
    }
}

