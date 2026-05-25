using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Core.Entities.PreSaleProyects;

namespace Infrastructure.Repositories
{
    public class PreSaleProyectsRepository : IPreSaleProyectsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public PreSaleProyectsRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, string? exclude = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM dbo.PRE_SALE_PROYECTS WHERE DESCRIPTION = @DESCRIPTION AND BUSINESS_ID = @BID");

                if (!string.IsNullOrEmpty(exclude))
                {
                    query.Append(" AND PRE_SALE_PROYECTS_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@DESCRIPTION", description);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (!string.IsNullOrEmpty(exclude)) cmd.Parameters.AddWithValue("@ID", exclude);


                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del proyecto.", ex.Message);
            }
        }

        public async Task<string> GetCodeAsync(long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_GET_LAST_PROYECT_CODE", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                object result = await cmd.ExecuteScalarAsync();
                return result == DBNull.Value || result == null ? null : result.ToString();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el último código de projecto.", ex.Message);
            }
        }





        public async Task AddAsync(PreSaleProyects entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PRE_SALE_PROYECTS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", entity.ContactsCrmId);
                cmd.Parameters.AddWithValue("@RESPONSIBLE_ID", entity.ResponsibleId == 0 ? DBNull.Value : (object)entity.ResponsibleId);
                cmd.Parameters.AddWithValue("@SUPERVISOR_ID", entity.SupervisorId == 0 ? DBNull.Value : (object)entity.SupervisorId);
                cmd.Parameters.AddWithValue("@SSOMA_ID", entity.SsomaId == 0 ? DBNull.Value : (object)entity.SsomaId);
                cmd.Parameters.AddWithValue("@TEC_LEADER_ID", entity.TecLeaderId == 0 ? DBNull.Value : (object)entity.TecLeaderId);
                cmd.Parameters.AddWithValue("@OPPORTUNITY_ID", entity.OpportunityId);
                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", entity.StatePreSaleId);
                cmd.Parameters.AddWithValue("@QUOTATION_NUMBER_ID", entity.QuotationNumberId == 0 ? DBNull.Value : (object)entity.QuotationNumberId);
                cmd.Parameters.AddWithValue("@ORDER_NUMBER_ID", entity.OrderNumberId == 0 ? DBNull.Value : (object)entity.OrderNumberId);
                cmd.Parameters.AddWithValue("@ORDER_DATE", !entity.OrderDate.HasValue ? DBNull.Value : (object)entity.OrderDate.Value);
                cmd.Parameters.AddWithValue("@START_DATE", entity.StartDate);
                cmd.Parameters.AddWithValue("@END_DATE", entity.EndDate);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader.GetInt32(reader.GetOrdinal("status"));
                    var message = reader.GetString(reader.GetOrdinal("message"));
                    if (status == 0)
                    {
                        throw new DatabaseException("Error reportado por la base de datos.", message);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el proyecto en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                if (ex is DatabaseException) throw;
                throw new DatabaseException("Error inesperado al guardar el proyecto.", ex.Message);
            }
        }

        public async Task<PagedResult<PreSaleProyects>> GetAllAsync(
        long businessId,
        string? search,
        int page,
        int pageSize,
        long? workerId,
        string? filterCode = null,
        string? filterProject = null,
        string? filterClient = null,
        string? filterSeller = null,
        string? filterResponsible = null,
        string? filterStatePreSale = null,
        string? filterStateOpportunity = null,
        string? filterFinishDate = null,
        DateTime? filterDateFrom = null,
        DateTime? filterDateTo = null,
        string? opporNum = null,
        long? usersId = null,
        string? sortBy = null,
        string? sortDirection = null,
        long? stateId = null,
        int? category = null,
        string? quoDate = null
)
        {
            try
            {
                var list = new List<PreSaleProyects>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_PRE_SALE_PROYECTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)workerId ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FILTER_CODE", (object?)(string.IsNullOrWhiteSpace(filterCode) ? null : filterCode.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_PROJECT", (object?)(string.IsNullOrWhiteSpace(filterProject) ? null : filterProject.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_CLIENT", (object?)(string.IsNullOrWhiteSpace(filterClient) ? null : filterClient.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_SELLER", (object?)(string.IsNullOrWhiteSpace(filterSeller) ? null : filterSeller.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_RESPONSIBLE", (object?)(string.IsNullOrWhiteSpace(filterResponsible) ? null : filterResponsible.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_STATE_PRESALE", (object?)(string.IsNullOrWhiteSpace(filterStatePreSale) ? null : filterStatePreSale.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_STATE_OPPORTUNITY", (object?)(string.IsNullOrWhiteSpace(filterStateOpportunity) ? null : filterStateOpportunity.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_FINISH_DATE", (object?)(string.IsNullOrWhiteSpace(filterFinishDate) ? null : filterFinishDate.Trim()) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FILTER_DATE_FROM", (object?)filterDateFrom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_DATE_TO", (object?)filterDateTo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OPPOR_NUM", (object?)(string.IsNullOrWhiteSpace(opporNum) ? null : opporNum.Trim()) ?? DBNull.Value); 
                cmd.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SORT_BY", (object?)(string.IsNullOrWhiteSpace(sortBy) ? null : sortBy.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SORT_DIRECTION", (object?)(string.IsNullOrWhiteSpace(sortDirection) ? null : sortDirection.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PROJECT_CATEGORY", (object?)category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@QUO_DATE", (object?)quoDate ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var opportunityId = reader.GetInt64(0);
                    list.Add(new PreSaleProyects
                    {
                        LinkToken = _linkTokenService.Issue("opportunity", opportunityId, TimeSpan.FromHours(1)),

                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                        Status = reader.GetString(3),

                        ClientsDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ResponsibleDescription = reader.IsDBNull(5) ? null : reader.GetString(5),

                        StatePreSaleDescription = reader.IsDBNull(6) ? null : reader.GetString(6),
                        StateColor = reader.IsDBNull(7) ? null : reader.GetString(7),

                        QuotationNumberDescription = reader.IsDBNull(8) ? null : reader.GetString(8),
                        OrderNumberDescription = reader.IsDBNull(9) ? null : reader.GetString(9),

                        FinishDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),

                        NumPercPro = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                        PreSaleProyectsCount = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),

                        StateGeneralDesc = reader.IsDBNull(13) ? null : reader.GetString(13),
                        StateGeneralColor = reader.IsDBNull(14) ? null : reader.GetString(14),

                        OpportunityStateDesc = reader.IsDBNull(15) ? null : reader.GetString(15),
                        OpportunityStateColor = reader.IsDBNull(16) ? null : reader.GetString(16),

                        OpportunityNumber = reader.IsDBNull(17) ? null : reader.GetString(17),

                        SellerDescription = reader.IsDBNull(18) ? null : reader.GetString(18),
                        ResponsibleId = reader.IsDBNull(19) ? null : reader.GetInt64(19),
                        StatePreSaleId = reader.IsDBNull(20) ? null : reader.GetInt64(20),
                        ContractObservationsPending = reader.IsDBNull(21) ? null : reader.GetInt32(21),
                        ProjectCategory = reader.IsDBNull(22) ? null : reader.GetInt32(22),
                        ContractTotalCount = reader.IsDBNull(23) ? null : reader.GetInt32(23),
                        CommercialTotalCount = reader.IsDBNull(24) ? null : reader.GetInt32(24),
                        ObsRevised = reader.IsDBNull(25) ? null : reader.GetInt32(25),
                        UnreadCommentsCount = reader.IsDBNull(26) ? null : reader.GetInt32(26),
                        StartDate = reader.IsDBNull(27) ? (DateTime?)null : reader.GetDateTime(27),
                        QuoDate = reader.IsDBNull(28) ? (DateTime?)null : reader.GetDateTime(28),
                        Category = reader.IsDBNull(29) ? null : reader.GetInt32(29),

                        SubTotal = reader.IsDBNull(30) ? null : reader.GetDecimal(30),
                        TotalAmount = reader.IsDBNull(31) ? null : reader.GetDecimal(31),
                        CostTotal = reader.IsDBNull(32) ? null : reader.GetDecimal(32),
                        CurrencyDesc = reader.IsDBNull(33) ? null : reader.GetString(33)
                    });
                }   

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<PreSaleProyects>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de proyectos paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PRE_SALE_PROYECTS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    items.Add(new OptionItem
                    {
                        Value = dr.GetInt64(0),
                        Label = dr.GetString(1)
                    });
                }

                var hasMore = items.Count > pageSize;
                if (hasMore) items.RemoveAt(items.Count - 1);

                return new PagedSelect<OptionItem>
                {
                    Items = items,
                    HasMore = hasMore,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener los proyectos para el selector.", ex.Message);
            }
        }


        public async Task<PreSaleProyects> GetByIdAsync(string LinkToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_PRE_SALE_PROYECTS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PRE_SALE_PROYECTS_ID", LinkToken);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var preSaleProjectId = reader.GetInt64(0);
                    return new PreSaleProyects
                    {
                        LinkToken = _linkTokenService.Issue("opportunity", preSaleProjectId, TimeSpan.FromHours(1)),

                        BusinessId = reader.IsDBNull(1) ? 0 : reader.GetInt64(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Status = reader.IsDBNull(3) ? null : reader.GetString(3),

                        ClientsId = reader.IsDBNull(4) ? 0 : reader.GetInt64(4),
                        ResponsibleId = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                        SupervisorId = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                        SsomaId = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                        TecLeaderId = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),

                        StatePreSaleId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        StateColor = reader.IsDBNull(10) ? null : reader.GetString(10),

                        FinishDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),

                        NumPercPro = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                        PreSaleProyectsCount = reader.IsDBNull(13) ? 0 : reader.GetInt32(13),

                        StateGeneralDesc = reader.IsDBNull(14) ? null : reader.GetString(14),
                        StateGeneralColor = reader.IsDBNull(15) ? null : reader.GetString(15),

                        OpportunityStateDesc = reader.IsDBNull(16) ? null : reader.GetString(16),
                        OpportunityStateColor = reader.IsDBNull(17) ? null : reader.GetString(17),

                        OpportunityNumber = reader.IsDBNull(18) ? null : reader.GetString(18),

                        SellerId = reader.IsDBNull(19) ? 0 : reader.GetInt64(19)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el proyecto por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(PreSaleProyects entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PRE_SALE_PROYECTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@PRE_SALE_PROYECTS_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", entity.ContactsCrmId);
                cmd.Parameters.AddWithValue("@RESPONSIBLE_ID", entity.ResponsibleId == 0 ? DBNull.Value : (object)entity.ResponsibleId);
                cmd.Parameters.AddWithValue("@SUPERVISOR_ID", entity.SupervisorId == 0 ? DBNull.Value : (object)entity.SupervisorId);
                cmd.Parameters.AddWithValue("@SSOMA_ID", entity.SsomaId == 0 ? DBNull.Value : (object)entity.SsomaId);
                cmd.Parameters.AddWithValue("@TEC_LEADER_ID", entity.TecLeaderId == 0 ? DBNull.Value : (object)entity.TecLeaderId);
                cmd.Parameters.AddWithValue("@OPPORTUNITY_ID", entity.OpportunityId);
                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", entity.StatePreSaleId);
                cmd.Parameters.AddWithValue("@QUOTATION_NUMBER_ID", entity.QuotationNumberId == 0 ? DBNull.Value : (object)entity.QuotationNumberId);
                cmd.Parameters.AddWithValue("@ORDER_NUMBER_ID", entity.OrderNumberId == 0 ? DBNull.Value : (object)entity.OrderNumberId);
                cmd.Parameters.AddWithValue("@ORDER_DATE", !entity.OrderDate.HasValue ? DBNull.Value : (object)entity.OrderDate.Value);
                cmd.Parameters.AddWithValue("@START_DATE", entity.StartDate);
                cmd.Parameters.AddWithValue("@END_DATE", entity.EndDate);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el proyecto en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(string LinkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PRE_SALE_PROYECTS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PRE_SALE_PROYECTS_ID", LinkToken);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del proyecto.", ex.Message);
            }
        }

        public async Task<PreSaleProyects> GetDetailAsync(string LinkToken, long businessId, CancellationToken ct = default)
        {
            try
            {
                // Helpers
                static int Ord(SqlDataReader rd, string name) => rd.GetOrdinal(name);
                static string? GetStr(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetString(i);
                static DateTime? GetDt(SqlDataReader rd, int i) => rd.IsDBNull(i) ? (DateTime?)null : rd.GetDateTime(i);
                static int? GetInt(SqlDataReader rd, int i) => rd.IsDBNull(i) ? (int?)null : rd.GetInt32(i);

                await using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync(ct);

                PreSaleProyects p;

                await using (var cmd = new SqlCommand("SP_WS_PRE_SALE_PROYECTS_DETAIL", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmd.Parameters.Add("@PRE_SALE_PROYECTS_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct))
                    {
                        if (!await r.ReadAsync(ct)) return null;

                        p = new PreSaleProyects
                        {
                            PreSaleProyectId = r.GetInt64(Ord(r, "PRE_SALE_PROYECTS_ID")),
                            LinkToken = _linkTokenService.Issue("projectsDetail", r.GetInt64("PRE_SALE_PROYECTS_ID"), TimeSpan.FromHours(1)),
                            BusinessId = r.GetInt64(Ord(r, "BUSINESS_ID")),
                            ProyectNum = r.GetString(Ord(r, "PROYECT_NUM")),
                            Description = r.GetString(Ord(r, "DESCRIPTION")),
                            ClientsDescription = GetStr(r, Ord(r, "CLIENTS_NAME")),
                            ResponsibleDescription = GetStr(r, Ord(r, "RESPONSIBLE_DESC")),
                            EndDate = r.GetDateTime(Ord(r, "END_DATE")),


                            ClientsSector = GetStr(r, Ord(r, "CLIENTS_SECTOR")),
                            PresupuestoEstimado = GetStr(r, Ord(r, "PRESUPUESTO_ESTIMADO")),
                            CreateDate = GetDt(r, Ord(r, "CREATE_DATE")),
                            StatePreSaleDescription = GetStr(r, Ord(r, "STATE_DESC")),
                            NumPercPro = GetInt(r, Ord(r, "NUM_PERC_PRO")),
                            ClientsRuc = GetStr(r, Ord(r, "CLIENTS_RUC")),
                            ClientsAddress = GetStr(r, Ord(r, "CLIENTS_ADDRESS")),
                            ClientsCity = GetStr(r, Ord(r, "CLIENTS_CITY")),
                            ClientsPhone = GetStr(r, Ord(r, "CLIENTS_PHONE")),
                            ClientsWeb = GetStr(r, Ord(r, "CLIENTS_WEB")),
                            ContactsCrmDescription = GetStr(r, Ord(r, "CONTACT_NAME")),
                            ContactJob = GetStr(r, Ord(r, "CONTACT_JOB")),
                            ContactPhone = GetStr(r, Ord(r, "CONTACT_PHONE")),
                            ContactEmail = GetStr(r, Ord(r, "CONTACT_EMAIL")),
                            ContactType = GetStr(r, Ord(r, "CONTACT_TYPE")),
                            CurrencyName = GetStr(r, Ord(r,"CURRENCY_NAME")),
                            ReasonRejection = GetStr(r,Ord(r,"REASON_REJECTION")),
                            TasksList = new List<PreSaleProjectTask>(),
                            HistoryChanges = new List<HistoryPreSaleProjectsChanges>(),
                            ActivityList = new List<PreSaleProjectActivity>(),
                            FiletrackingList = new List<ProjectFiletracking>()


                        };
                    }
                }


                await using (var cmdTask = new SqlCommand("SP_WS_PRE_SALE_PROYECTS_TASK", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdTask.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdTask.Parameters.Add("@PROJECT_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r2 = await cmdTask.ExecuteReaderAsync(ct);
                    if (r2.HasRows)
                    {
                        int cId = Ord(r2, "TASKS_ID");
                        int cTitle = Ord(r2, "TITLE");
                        int cDesc = Ord(r2, "DESCRIPTION");
                        int cPriorityColor = Ord(r2, "PRIORITY_COLOR");
                        int cPriorityId = Ord(r2, "PRIORITY_STATE_ID");
                        int cPriority = Ord(r2, "PRIORITY_DESC");
                        int cResp = Ord(r2, "STATE_DESC");
                        int cStatus = Ord(r2, "STATE_COLOR");
                        int cStatusProgress = Ord(r2, "NUM_PERC_PRO");
                        int cWorker = Ord(r2, "WORKER");
                        int cEnd = Ord(r2, "END_DATE");

                        while (await r2.ReadAsync(ct))
                        {
                            int? priorityId = r2.IsDBNull(cPriorityId) ? (int?)null : r2.GetInt32(cPriorityId);

                            p.TasksList.Add(new PreSaleProjectTask
                            {
                                TasksToken = _linkTokenService.Issue("tasks", r2.GetInt64(cId), TimeSpan.FromHours(1)),
                                TitleTasks = GetStr(r2, cTitle),
                                DescTasks = GetStr(r2, cDesc),
                                PriorityToken = priorityId.HasValue ? _linkTokenService.Issue("priority", priorityId.Value, TimeSpan.FromHours(1)) : null,
                                PriorityColor = GetStr(r2, cPriorityColor),
                                PriorityDesc = GetStr(r2, cPriority),
                                TasksResp = GetStr(r2, cWorker),
                                StatusTasks = GetStr(r2, cResp),
                                StatusProgress = r2.GetInt32(cStatusProgress),
                                StateColor = GetStr(r2, cStatus),
                                EndRegister = r2.IsDBNull(cEnd) ? (DateTime?)null : r2.GetDateTime(cEnd)
                            });
                        }
                    }
                }

                await using (var cmdHistory = new SqlCommand("SP_CHANGE_HISTORY_PROJECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdHistory.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdHistory.Parameters.Add("@ENTITY_PROJ_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r6 = await cmdHistory.ExecuteReaderAsync(ct);
                    if (r6.HasRows)
                    {

                        int cHistory = Ord(r6, "HISTORY");
                        int cUsersChange = Ord(r6, "USERS_CHANGE");
                        int cDateChange = Ord(r6, "CHANGED_AT");

                        while (await r6.ReadAsync(ct))
                        {
                            p.HistoryChanges.Add(new HistoryPreSaleProjectsChanges
                            {

                                History = r6.IsDBNull(cHistory) ? null : r6.GetString(cHistory),
                                UsersName = r6.IsDBNull(cUsersChange) ? null : r6.GetString(cUsersChange),
                                DateChange = r6.IsDBNull(cDateChange) ? (DateTime?)null : r6.GetDateTime(cDateChange)
                            });
                        }
                    }
                }

                await using (var cmdAct = new SqlCommand("SP_WS_PROJECT_ACTIVITY", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdAct.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdAct.Parameters.Add("@PROJECT_ID", SqlDbType.BigInt).Value = LinkToken;
                    cmdAct.Parameters.Add("@IND_ENTITY", SqlDbType.BigInt).Value = 2L;

                    await using var r4 = await cmdAct.ExecuteReaderAsync(ct);
                    if (r4.HasRows)
                    {
                        int cId = Ord(r4, "ACTIVITY_ID");
                        int cActivitys = Ord(r4, "ACTIVITY_MESSAGE");
                        int cActivitysAdd = Ord(r4, "MESSAGE_ADIC");
                        int cCreateDate = Ord(r4, "FINISH_DATE");
                        int cWorkerName = Ord(r4, "WORKER_SENDER");
                        int cStateColor = Ord(r4, "STATE_COLOR");
                        int cStateDesc = Ord(r4, "STATE_DESC");
                        int cActivityIcon = Ord(r4, "ACTIVITY_ICON");
                        int cActivity = Ord(r4, "ACTIVITY");
                        int cActivityColor = Ord(r4, "COLOR");
                        int cActivityPriority = Ord(r4, "PRIORITY_DESC");
                        while (await r4.ReadAsync(ct))
                        {
                            p.ActivityList.Add(new PreSaleProjectActivity
                            {
                                LinkToken = _linkTokenService.Issue("activity", r4.GetInt64(cId), TimeSpan.FromHours(1)),
                                Activitys = GetStr(r4, cActivitys),
                                MessageAddition = GetStr(r4, cActivitysAdd),
                                DateActivity = r4.IsDBNull(cCreateDate) ? (DateTime?)null : r4.GetDateTime(cCreateDate),
                                workerName = GetStr(r4, cWorkerName),
                                ActivityState = GetStr(r4, cStateDesc),
                                ActivityStateColor = GetStr(r4, cStateColor),
                                ActivityIcon = GetStr(r4, cActivityIcon),
                                Activity = GetStr(r4, cActivity),
                                ActivityPriority = GetStr(r4, cActivityPriority),
                                ActivityPriorityColor = GetStr(r4, cActivityColor)


                            });
                        }
                    }
                }

                await using (var cmdFiles = new SqlCommand("SP_WS_PROJECT_FILE_TRACKING", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdFiles.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdFiles.Parameters.Add("@PROJECT_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r5 = await cmdFiles.ExecuteReaderAsync(ct);
                    if (r5.HasRows)
                    {
                        int cId = Ord(r5, "FILE_TRACKING_ID");
                        int cUrl = Ord(r5, "FILE_URL");
                        int cComment = Ord(r5, "COMMENT");
                        int cDate = Ord(r5, "CREATE_DATE");
                        int cCodeProject = Ord(r5, "PROYECT_NUM");
                        int cTitle = Ord(r5, "FILE_TITLE");
                        int cRelativePath = Ord(r5, "RELATIVE_PATH");
                        int cArchiveType = Ord(r5, "FILE_TYPE");

                        while (await r5.ReadAsync(ct))
                        {
                            p.FiletrackingList.Add(new ProjectFiletracking
                            {
                                LinkToken = _linkTokenService.Issue("fileTracking", r5.GetInt64(cId), TimeSpan.FromHours(1)),
                                FileUrl = GetStr(r5, cUrl),
                                CommentFile = GetStr(r5, cComment),
                                DateUpload = r5.IsDBNull(cDate) ? (DateTime?)null : r5.GetDateTime(cDate),
                                CodeProject = GetStr(r5, cCodeProject),
                                FileTitle = GetStr(r5, cTitle),
                                RelativePath = GetStr(r5, cRelativePath),
                                ArchiveType = GetStr(r5, cArchiveType)
                            });
                        }
                    }
                }



                return p;
            }

            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el detalle del proyecto.", ex.Message);
            }
        }

        public async Task<bool> UpdateResponsibleAsync(PreSaleProyects entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_RESPONSIBLE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.OpportunityId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.ResponsibleId);
                cmd.Parameters.AddWithValue("@PROJECT_CATEGORY", entity.ProjectCategory);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);


                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el responsable del proyecto.", ex.Message);

            }
        }


        public async Task<bool> UpdateStateAsync(PreSaleProyects entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PROJECT_STATE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", entity.StatePreSaleId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);
                cmd.Parameters.AddWithValue("@OBS_TYPE", (object?)entity.ObsType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OBS_SEVERITY", (object?)entity.ObsSeverity ?? DBNull.Value);

                if (entity.ObsReason != null && entity.ObsReason.Any())
                {
                    string jsonReasons = System.Text.Json.JsonSerializer.Serialize(entity.ObsReason);
                    cmd.Parameters.AddWithValue("@OBS_REASON", jsonReasons);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@OBS_REASON", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@OBS_STATUS_ID", (object?)entity.ObsStatusId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DUE_DATE", (object?)entity.ObsDueDate ?? DBNull.Value);

                if (entity.AssignedWorkerId != null && entity.AssignedWorkerId.Any())
                {
                    string jsonWorkers = System.Text.Json.JsonSerializer.Serialize(entity.AssignedWorkerId);
                    cmd.Parameters.AddWithValue("@ASSIGNED_WORKER_ID", jsonWorkers);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ASSIGNED_WORKER_ID", DBNull.Value);
                }

                var tvpFile = BuildPreSaleProyectFilesTvp(entity.PreSaleProyectFiles ?? new List<PreSaleProyectFile>());

                var pFiles = cmd.Parameters.Add("@PROJECT_FILES", SqlDbType.Structured);
                pFiles.TypeName = "dbo.TT_FILE_TRACKING"; 
                pFiles.Value = tvpFile;


                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    var status = reader["status"] != DBNull.Value ? Convert.ToInt32(reader["status"]) : 0;
                    return status == 1;
                }

                return false;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar estado.", ex.Message);
            }
        }


        private DataTable BuildPreSaleProyectFilesTvp(List<PreSaleProyectFile> files)
        {
            var table = new DataTable();

            table.Columns.Add("FILE_TRACKING_ID", typeof(int));
            table.Columns.Add("FILE_TITLE", typeof(string));
            table.Columns.Add("FILE_URL", typeof(string));
            table.Columns.Add("RELATIVE_PATH", typeof(string));
            table.Columns.Add("ARCHIVE_TYPE", typeof(string)); 

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file.FileUrl)) continue;

                    table.Rows.Add(
                        0,                               
                        file.FileTitle ?? "",              
                        file.FileUrl ?? "",                
                        file.RelativePath ?? "",           
                        file.ArchiveType ?? ""             
                    );
                }
            }

            return table;
        }
    }


}