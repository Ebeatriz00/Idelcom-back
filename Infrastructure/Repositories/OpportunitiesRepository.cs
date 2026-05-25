using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories
{
    public class OpportunitiesRepository : IOpportunitiesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public OpportunitiesRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string oppDesc, long businessId, string? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM OPPORTUNITY WHERE OPPOR_DESC = @DESC  AND BUSINESS_ID = @BID";

                if (!string.IsNullOrEmpty(excludeId))
                    query += " AND OPPOR_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DESC", oppDesc);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (!string.IsNullOrEmpty(excludeId)) cmd.Parameters.AddWithValue("@ID", excludeId);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la oportunidad.", ex.Message);
            }
        }
        public async Task<string> GetCodeAsync(long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_GET_LAST_OPPORTUNITY_CODE", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                object result = await cmd.ExecuteScalarAsync();
                return result == DBNull.Value || result == null ? null : result.ToString();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el último código de oportunidad.", ex.Message);
            }
        }

        public async Task<GlobalResponse> AddAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_REGISTER_OPPORTUNITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@NEG_STAGES_ID", entity.NegotiationStagesId);
                cmd.Parameters.AddWithValue("@OPPOR_DESC", entity.OpporDesc);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@BUSINESS_LINE_ID", entity.BusinessLineId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@PORCENT_PROGRESS_PRO", (object?)entity.PorcentProgressPro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CURRENCY_ID", entity.CurrencyId);
                cmd.Parameters.AddWithValue("@OPPOR_AMOUNT", entity.OpporAmount);
                cmd.Parameters.AddWithValue("@REGISTRATION_DATE", entity.DateRegister);
                cmd.Parameters.AddWithValue("@FINISH_DATE", (object?)entity.DateFinish ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CONSULT_DATE", (object?)entity.ConsultDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@QUO_DATE", (object?)entity.QuoDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FOLLOWUP_ENABLED", (object?)entity.FollowupEnabled ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FOLLOWUP_EVERY_DAYS", (object?)entity.FollowupEveryDay ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);
                cmd.Parameters.AddWithValue("@REQUIRES_LICITATION", entity.IsHiring);
                cmd.Parameters.AddWithValue("@PROCESS_TYPE", 1); // CONSULTORIA
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", entity.ContactsId);
                cmd.Parameters.AddWithValue("@PM_CONDITION_ID", entity.PmConditionId);
                cmd.Parameters.AddWithValue("@FLOW_TYPE_ID", entity.FlowTypeId);
                cmd.Parameters.AddWithValue("@TYPE_OPPOR", entity.TypeOppor);
                cmd.Parameters.AddWithValue("@PARENT_OPPORTUNITY_ID", (object?)entity.ParentOpporId ?? DBNull.Value);

                // TVP DELIVERABLES
                var tvp = BuildDeliverablesTvp(entity.DeliverablesHiring ?? new List<DeliverablesOppor>());
                var pDeliverables = cmd.Parameters.Add("@DELIVERABLES_HIRING", SqlDbType.Structured);
                pDeliverables.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                pDeliverables.Value = tvp;
                
                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    throw new DatabaseException("Error al registrar en base de datos.", "El procedimiento no devolvió resultados.");

                var status = Convert.ToInt32(reader["status"]);
                var message = reader["message"]?.ToString() ?? string.Empty;

                long? opporId = reader["oppor_id"] == DBNull.Value ? null : Convert.ToInt64(reader["oppor_id"]);
                string? opporNum = reader["oppor_num"] == DBNull.Value ? null : reader["oppor_num"]?.ToString();

                return new GlobalResponse
                {
                    Status = status,
                    Message = message,
                    Id = opporId,
                    OpporNum = opporNum
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la oportunidad en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la oportunidad.", ex.Message);
            }
        }

        public async Task<GlobalResponse> AttachHiringFilesAsync(long businessId, long opporId, long updateUser, List<OpportFiletracking> files)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OPPOR_ATTACH_HIRING_FILES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", updateUser);
                cmd.Parameters.AddWithValue("@FILE_TYPE", "CONSULTORIA");

                var tvpFile = BuildDeliverablesHiringFileTvp(files ?? new List<OpportFiletracking>());
                var pFiles = cmd.Parameters.Add("@DELIVERABLES_HIRING_FILE", SqlDbType.Structured);
                pFiles.TypeName = "dbo.TT_FILE_TRACKING";
                pFiles.Value = tvpFile;

                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    throw new DatabaseException("Error al adjuntar archivos.", "El procedimiento no devolvió resultados.");

                return new GlobalResponse
                {
                    Status = Convert.ToInt32(reader["status"]),
                    Message = reader["message"]?.ToString() ?? ""
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al adjuntar archivos en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<Opportunities>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersId, long? stateId, long? workerId, DateTime? filterStartDate, DateTime? filterFinishDate, int? filterYear)
        {
            try
            {
                var list = new List<Opportunities>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_OPPORTUNITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_START_DATE", (object?)filterStartDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_FINSH_DATE", (object?)filterFinishDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILTER_YEAR", (object?)filterYear ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)workerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(0);

                    list.Add(new Opportunities
                    {
                        LinkToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        OpporNumber = reader.GetString(2),
                        DateRegister = reader.GetDateTime(3),
                        OpporDesc = reader.GetString(4),
                        ClientsName = reader.GetString(5),
                        SalesName = reader.IsDBNull(6) ? null : reader.GetString(6),
                        SalesPre = reader.IsDBNull(7) ? null : reader.GetString(7),
                        DateFinish = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        PorcentProgressPro = reader.GetInt32(9),
                        StateOpporDesc = reader.GetString(10),
                        StateColor = reader.GetString(11),
                        Status = reader.GetString(12),
                        Tasks = reader.GetInt32(13),
                        CommentsCount = reader.GetInt32(14),
                        unreadCommentsCount = reader.GetInt32(15),
                        StateGeneral = reader.GetString(17),
                        ColorState = reader.GetString(18),
                        DeliverablesCount = reader.GetInt32(19),
                        IsOpporManager = reader.IsDBNull(20) ? null : reader.GetInt64(20),
                        FollowupEnabled = reader.IsDBNull(21) ? null : reader.GetBoolean(21),
                        FollowupNextAt = reader.IsDBNull(22) ? null : reader.GetDateTime(22),
                        FollowupSuspended = reader.IsDBNull(23) ? null : reader.GetBoolean(23),
                        NegotiationStagesDesc = reader.GetString(24),
                        IsHiring = reader.IsDBNull(25) ? null : reader.GetBoolean(25),
                        ObsNotResolved = reader.IsDBNull(26) ? null : reader.GetInt32(26),
                        ObsNotApproved = reader.IsDBNull(27) ? null : reader.GetInt32(27),
                        ObsApproved = reader.IsDBNull(28) ? null : reader.GetInt32(28),
                        ObsNotDate  = reader.IsDBNull(29) ? null : reader.GetInt32(29),
                        PreSalesDelivered = reader.IsDBNull(30) ? null : reader.GetInt32(30),
                        LicDocDelivered = reader.IsDBNull(31) ? null : reader.GetInt32(31),
                        LicConsultDelivered = reader.IsDBNull(32) ? null : reader.GetInt32(32),
                        GoesToPreSales = reader.IsDBNull(33) ? null : reader.GetInt32(33),
                        CanChangeStateByDelivery = reader.IsDBNull(34) ? null : reader.GetInt32(34),
                        TypeObsClientsId = reader.IsDBNull(35) ? null : reader.GetInt32(35),
                        TypeObsEconomic = reader.IsDBNull(36) ? null : reader.GetInt32(36),

                        ObsNotResolvedPre = reader.IsDBNull(37) ? null : reader.GetInt32(37),
                        ObsNotApprovedPre = reader.IsDBNull(38) ? null : reader.GetInt32(38),
                        ObsApprovedPre = reader.IsDBNull(39) ? null : reader.GetInt32(39),
                        ObsNotDatePre = reader.IsDBNull(40) ? null : reader.GetInt32(40),
                        ObsQuo = reader.IsDBNull(41) ? null : reader.GetInt32(41),

                        ObsNotResolvedLic = reader.IsDBNull(42) ? null : reader.GetInt32(42),
                        ObsNotApprovedLic = reader.IsDBNull(43) ? null : reader.GetInt32(43),
                        ObsApprovedLic = reader.IsDBNull(44) ? null : reader.GetInt32(44),
                        ObsNotDateLic = reader.IsDBNull(45) ? null : reader.GetInt32(45),
                        ExistQuo = reader.IsDBNull(46) ? null : reader.GetInt32(46),

                        StatePresales = reader.IsDBNull(47) ? null : reader.GetString(47),
                        colorStatePresales = reader.IsDBNull(48) ? null : reader.GetString(48),
                        ObsQuoResolved = reader.IsDBNull(49) ? null : reader.GetInt32(49),

                        TypeOpporDesc = reader.IsDBNull(50) ? null : reader.GetString(50),
                        TotalAmount = reader.IsDBNull(51) ? null : reader.GetDecimal(51),
                        CurrencyDesc = reader.IsDBNull(52) ? null : reader.GetString(52),
                        QuoDate = reader.IsDBNull(53) ? null : reader.GetDateTime(53)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Opportunities>
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

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, long clientsId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OPPORTUNTIES_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);
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
                throw new DatabaseException("Error al obtener los cliente para el selector.", ex.Message);
            }
        }

      
        public async Task<Opportunities> GetByIdAsync(string LinkToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_OPPORTUNITY_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_ID", LinkToken);
                using var reader = await cmd.ExecuteReaderAsync();

                Opportunities? result = null;

                if (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(0);
                    result = new Opportunities 
                    {
                        LinkToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        OpporNumber = reader.GetString(2),
                        OpporDesc = reader.GetString(3),
                        ClientsId = reader.GetInt64(4),
                        BusinessLineId = reader.GetInt64(5),
                        WorkerId = reader.IsDBNull(6) ? null : reader.GetInt64(6),
                        PorcentProgressPro = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                        StateOpporId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                        CurrencyId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        OpporAmount = reader.IsDBNull(10) ? null : reader.GetDecimal(10),
                        DateRegister = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                        DateFinish = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                        ConsultDate = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                        QuoDate = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                        ViabilityScore = reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
                        Compliance = reader.IsDBNull(16) ? 0 : reader.GetInt32(16),
                        PartialCompliance = reader.IsDBNull(17) ? 0 : reader.GetInt32(17),
                        NonCompliance = reader.IsDBNull(18) ? 0 : reader.GetInt32(18),

                        Authority = reader.IsDBNull(19) ? 0 : reader.GetInt32(19),
                        AuthorityDesc = reader.IsDBNull(20) ? null : reader.GetString(20),

                        Budget = reader.IsDBNull(21) ? 0 : reader.GetInt32(21),
                        BudgetDesc = reader.IsDBNull(22) ? null : reader.GetString(22),

                        Need = reader.IsDBNull(23) ? 0 : reader.GetInt32(23),
                        NeedDesc = reader.IsDBNull(24) ? null : reader.GetString(24),

                        Term = reader.IsDBNull(25) ? 0 : reader.GetInt32(25),
                        TermDesc = reader.IsDBNull(26) ? null : reader.GetString(26),

                        CompanyExperience = reader.IsDBNull(27) ? 0 : reader.GetInt32(27),
                        CompanyExperienceDesc = reader.IsDBNull(28) ? null : reader.GetString(28),

                        WorkerExperience = reader.IsDBNull(29) ? 0 : reader.GetInt32(29),
                        WorkerExperienceDesc = reader.IsDBNull(30) ? null : reader.GetString(30),

                        StaffExperience = reader.IsDBNull(31) ? 0 : reader.GetInt32(31),
                        StaffExperienceDesc = reader.IsDBNull(32) ? null : reader.GetString(32),

                        Ability = reader.IsDBNull(33) ? 0 : reader.GetInt32(33),
                        AbilityDesc = reader.IsDBNull(34) ? null : reader.GetString(34),

                        Shedule = reader.IsDBNull(35) ? 0 : reader.GetInt32(35),
                        SheduleDesc = reader.IsDBNull(36) ? null : reader.GetString(36),
                        IsAprovedViability = reader.IsDBNull(37) ? null : reader.GetBoolean(37),
                        IsPreOpportunity = reader.IsDBNull(38) ? null : reader.GetBoolean(38),
                        DecisionManager = reader.IsDBNull(39) ? null : reader.GetString(39),
                        RequiresIsos = reader.IsDBNull(40) ? 0 : reader.GetInt32(40),
                        RequiresIsosDesc = reader.IsDBNull(41) ? null : reader.GetString(41),
                        ContractMethod = reader.IsDBNull(42) ? 0 : reader.GetInt32(42),
                        ContractMethodDesc = reader.IsDBNull(43) ? null : reader.GetString(43),
                        NegotiationStagesId = reader.GetInt64(44),
                        IsHiring = reader.IsDBNull(45) ? null : reader.GetBoolean(45),
                        IsReEvaluation = reader.IsDBNull(46) ? null : reader.GetBoolean(46),
                        BrandAproach = reader.IsDBNull(47) ? 0 : reader.GetInt32(47),
                        BrandAproachDesc = reader.IsDBNull(48) ? null : reader.GetString(48),
                        TechnicalChanges = reader.IsDBNull(49) ? 0 : reader.GetInt32(49),
                        TechnicalChangesDesc = reader.IsDBNull(50) ? null : reader.GetString(50),
                        FollowupEnabled = reader.IsDBNull(51) ? null : reader.GetBoolean(51),
                        FollowupEveryDay = reader.IsDBNull(52) ? null : reader.GetInt32(52),
                        ContactsId = reader.IsDBNull(53) ? null : reader.GetInt64(53),
                        FlowTypeId = reader.IsDBNull(54) ? null : reader.GetInt64(54),
                        PmConditionId = reader.IsDBNull(55) ? null : reader.GetInt64(55),
                        TypeOppor = reader.IsDBNull(56) ? null : reader.GetInt32(56),
                        ParentOpporId = reader.IsDBNull(57) ? null : reader.GetInt64(57),

                        DeliverablesHiring = new List<DeliverablesOppor>(),
                        HiringFiles =  new List<OpportFiletracking>(),
                    };
                }

                if (result == null) return null;

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        long? tasksId = reader.IsDBNull(4) ? (long?)null : reader.GetInt64(4);
                        result.DeliverablesHiring!.Add(new DeliverablesOppor
                        {
                            DeliverablesId = reader.GetInt64(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Comment = reader.IsDBNull(2) ? null : reader.GetString(2),
                            DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            TasksId = tasksId,
                            TaskToken = _linkTokenService.Issue("tasks", Convert.ToInt64(tasksId), TimeSpan.FromHours(1)),
                            State = reader.IsDBNull(5) ? null : reader.GetString(5),
                            TaskStateDesc = reader.IsDBNull(5) ? null : reader.GetString(5),
                            TaskStateColor = reader.IsDBNull(6) ? null : reader.GetString(6),
                            NumPercPro = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            RequestId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                            TypeDeliverable = reader.IsDBNull(9) ? null : reader.GetInt32(9)

                        });
                    }
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        
                        result.HiringFiles!.Add(new OpportFiletracking
                        {
                            LinkToken = reader.GetString(0),
                            ArchiveType = reader.IsDBNull(1) ? null : reader.GetString(1),
                            FileUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                            CommentFile = reader.IsDBNull(3) ? null : reader.GetString(3),
                            FileTitle = reader.IsDBNull(4) ? null : reader.GetString(4),
                            RelativePath = reader.IsDBNull(5) ? null : reader.GetString(5),

                        });
                    }
                }

                return result;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener al cliente por ID.", ex.Message);
            }
        }

        public async Task<GlobalResponse> UpdateAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPORTUNITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("NEG_STAGES_ID", entity.NegotiationStagesId);
                cmd.Parameters.AddWithValue("@OPPOR_DESC", entity.OpporDesc);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@BUSINESS_LINE_ID", entity.BusinessLineId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@PORCENT_PROGRESS_PRO", entity.PorcentProgressPro);
                cmd.Parameters.AddWithValue("@CURRENCY_ID", entity.CurrencyId);
                cmd.Parameters.AddWithValue("@OPPOR_AMOUNT", entity.OpporAmount);
                cmd.Parameters.AddWithValue("@REGISTRATION_DATE", entity.DateRegister);
                cmd.Parameters.AddWithValue("@FINISH_DATE", entity.DateFinish);
                cmd.Parameters.AddWithValue("@CONSULT_DATE", (object?)entity.ConsultDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@QUO_DATE", (object?)entity.QuoDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                cmd.Parameters.AddWithValue("@FOLLOWUP_ENABLED", (object?)entity.FollowupEnabled ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FOLLOWUP_EVERY_DAYS", (object?)entity.FollowupEveryDay ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@REQUIRES_LICITATION", entity.IsHiring);
                cmd.Parameters.AddWithValue("@PROCESS_TYPE", 1); // CONSULTORIA
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", entity.ContactsId);

                cmd.Parameters.AddWithValue("@PM_CONDITION_ID", entity.PmConditionId);
                cmd.Parameters.AddWithValue("@FLOW_TYPE_ID", entity.FlowTypeId);

                cmd.Parameters.AddWithValue("@TYPE_OPPOR", entity.TypeOppor);
                cmd.Parameters.AddWithValue("@PARENT_OPPORTUNITY_ID", (object?)entity.ParentOpporId ?? DBNull.Value);


                var tvp = BuildDeliverablesTvp(entity.DeliverablesHiring ?? new List<DeliverablesOppor>());
                var pDeliverables = cmd.Parameters.Add("@DELIVERABLES_HIRING", SqlDbType.Structured);
                pDeliverables.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                pDeliverables.Value = tvp;

                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    throw new DatabaseException("Error al registrar en base de datos.", "El procedimiento no devolvió resultados.");

                var status = Convert.ToInt32(reader["status"]);
                var message = reader["message"]?.ToString() ?? string.Empty;

                long? opporId = reader["oppor_id"] == DBNull.Value ? null : Convert.ToInt64(reader["oppor_id"]);
                string? opporNum = reader["oppor_num"] == DBNull.Value ? null : reader["oppor_num"]?.ToString();

                return new GlobalResponse
                {
                    Status = status,
                    Message = message,
                    Id = opporId,
                    OpporNum = opporNum
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar al cliente en base de datos.", ex.Message);
            }
        }


        public async Task<bool> PatchStatusAsync(string LinkToken, string status, long userBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPORTUNITY_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_ID", LinkToken);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", userBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del cliente.", ex.Message);
            }
        }

        public async Task<Opportunities?> GetDetailAsync(string LinkToken, long businessId, long? usersId, CancellationToken ct = default)
        {
            try
            {
                // Helpers a nivel de método (los puedes dejar así)
                static int Ord(SqlDataReader rd, string name) => rd.GetOrdinal(name);
                static string? GetStr(SqlDataReader rd, int i) => rd.IsDBNull(i) ? null : rd.GetString(i);
                static int? GetInt(SqlDataReader rd, int i) => rd.IsDBNull(i) ? (int?)null : rd.GetInt32(i);
                static DateTime? GetDt(SqlDataReader rd, int i) => rd.IsDBNull(i) ? (DateTime?)null : rd.GetDateTime(i);

                await using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync(ct);

                Opportunities o;

                // ---------- 1) DETALLE PRINCIPAL ----------
                await using (var cmd = new SqlCommand("SP_WS_OPPOR_DETAIL", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmd.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = LinkToken;

                    // BLOQUE AISLADO: r se dispone al cerrar estas llaves
                    await using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct))
                    {
                        if (!await r.ReadAsync(ct)) return null;

                        o = new Opportunities
                        {
                            BusinessId = r.GetInt64(Ord(r, "BUSINESS_ID")),
                            LinkToken = _linkTokenService.Issue("opportunityDetail", r.GetInt64("OPPOR_ID"), TimeSpan.FromHours(1)),
                            OpporNumber = r.GetString(Ord(r, "OPPOR_NUM")),
                            OpporDesc = r.GetString(Ord(r, "OPPOR_DESC")),
                            ClientsDocument = GetStr(r, Ord(r, "DOCUMENTS")),
                            ClientsName = GetStr(r, Ord(r, "CLIENTS_NAME")),
                            ClientsAddress = GetStr(r, Ord(r, "CLIENT_ADDRESS")),
                            ClientsSector = GetStr(r, Ord(r, "DESC_SECTOR")),
                            DepartmentName = GetStr(r, Ord(r, "DEPARTAMENT_NAME")),
                            ClientsPhone = GetStr(r, Ord(r, "CLIENT_PHONE")),
                            ClientsWeb = GetStr(r, Ord(r, "WEB")),
                            SalesName = GetStr(r, Ord(r, "COMMERCIAL")),
                            DateFinish = GetDt(r, Ord(r, "FINISH_DATE")),
                            StateStatusProject = GetStr(r, Ord(r, "STATE_DESC")),
                            NumPercPro = GetInt(r, Ord(r, "NUM_PERC_PRO")),
                            DescLineBusiness = GetStr(r, Ord(r, "DESC_LINE")),
                            PorcentProgressPro = GetInt(r, Ord(r, "PORCENT_PROGRESS_PRO")),
                            OpporAmountStr = GetStr(r, Ord(r, "OPPOR_AMOUNT")),
                            DateRegister = GetDt(r, Ord(r, "REGISTRATION_DATE")),
                            ContactsName = GetStr(r, Ord(r, "CONTACT_NAME")),
                            ContactsJob = GetStr(r, Ord(r, "JOB_TITLE")),
                            ContactsPhone = GetStr(r, Ord(r, "PHONE")),
                            ContactsEmail = GetStr(r, Ord(r, "EMAIL")),
                            ContactsType = GetStr(r, Ord(r, "CONTACT_TYPE")),
                            StateProject = GetStr(r, Ord(r, "STATE_PROJECT")),
                            PorcentProgressAdv = GetInt(r, Ord(r, "PORCENT_PROGRESS_ADVANCE_RAW")),
                            PorcentProgressTxt = GetStr(r, Ord(r, "PORCENT_PROGRESS_ADVANCE_TXT")),
                            WorkerResp = GetStr(r, Ord(r, "WORKER_RESPONSABLE")),
                            FinishDateProject = GetDt(r, Ord(r, "PROJECT_FINISH_DATE_RAW")),
                            FinishDateProjectTxt = GetStr(r, Ord(r, "PROJECT_FINISH_DATE_TXT")),
                            ReasonRejection = GetStr(r, Ord(r,"REASON_REJECTION")),
                            TasksList = new List<OpportTask>(),
                            ProjectTeamList = new List<OpportProjectTeam>(),
                            ActivityList = new List<OpportActivity>(),
                            FiletrackingList = new List<OpportFiletracking>(),
                            HistoryChanges = new List<HistoryOpportunityChanges>()
                        };
                    }
                }

                // ---------- 2) TAREAS ----------
                await using (var cmdTask = new SqlCommand("SP_WS_OPPOR_TASK", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdTask.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdTask.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = LinkToken;
                    cmdTask.Parameters.AddWithValue("@WORKER_ID", (object?)usersId ?? DBNull.Value);

                    await using var r2 = await cmdTask.ExecuteReaderAsync(ct);
                    if (r2.HasRows)
                    {
                        int cId = Ord(r2, "TASKS_ID");
                        int cTitle = Ord(r2, "TITLE");
                        int cDesc = Ord(r2, "DESCRIPTION");
                        int cPriorityColor = Ord(r2, "PRIORITY_COLOR");
                        int cPriorityId = Ord(r2, "PRIORITY_ID");
                        int cPriority = Ord(r2, "PRIORITY_DESC");
                        int cResp = Ord(r2, "STATE_DESC");
                        int cStatus = Ord(r2, "STATE_COLOR");
                        int cStatusProgress = Ord(r2, "NUM_PERC_PRO");
                        int cWorker = Ord(r2, "WORKER");
                        int cEnd = Ord(r2, "END_DATE");

                        while (await r2.ReadAsync(ct))
                        {
                            int? priorityId = r2.IsDBNull(cPriorityId) ? (int?)null : r2.GetInt32(cPriorityId);

                            o.TasksList.Add(new OpportTask
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

                // ---------- 3) EQUIPO DEL PROYECTO ----------
                await using (var cmdTeam = new SqlCommand("SP_WS_OPPOR_PROJECT_TEAM", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdTeam.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdTeam.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r3 = await cmdTeam.ExecuteReaderAsync(ct);
                    if (r3.HasRows)
                    {
                        int cTeam = Ord(r3, "TEAM");
                        while (await r3.ReadAsync(ct))
                        {
                            o.ProjectTeamList.Add(new OpportProjectTeam
                            {
                                TeamMember = GetStr(r3, cTeam)
                            });
                        }
                    }
                }

                // ---------- 4) ACTIVIDADES ----------
                await using (var cmdAct = new SqlCommand("SP_WS_OPPOR_ACTIVITY", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdAct.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdAct.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = LinkToken;
                    cmdAct.Parameters.Add("@IND_ENTITY", SqlDbType.BigInt).Value = 1L;
                    cmdAct.Parameters.AddWithValue("@WORKER_ID", (object?)usersId ?? DBNull.Value);

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
                            o.ActivityList.Add(new OpportActivity
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

                // ---------- 5) ARCHIVOS DE SEGUIMIENTO ----------
                await using (var cmdFiles = new SqlCommand("SP_WS_OPPOR_FILE_TRACKING", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdFiles.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdFiles.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r5 = await cmdFiles.ExecuteReaderAsync(ct);
                    if (r5.HasRows)
                    {
                        int cId = Ord(r5, "FILE_TRACKING_ID");
                        int cUrl = Ord(r5, "FILE_URL");
                        int cComment = Ord(r5, "COMMENT");
                        int cDate = Ord(r5, "CREATE_DATE");
                        int cCodeOppor = Ord(r5, "OPPOR_NUM");
                        int cTitle = Ord(r5, "FILE_TITLE");
                        int cRelativePath = Ord(r5, "RELATIVE_PATH");
                        int cArchiveType = Ord(r5, "FILE_TYPE");

                        while (await r5.ReadAsync(ct))
                        {
                            o.FiletrackingList.Add(new OpportFiletracking
                            {
                                LinkToken = _linkTokenService.Issue("fileTracking", r5.GetInt64(cId), TimeSpan.FromHours(1)),
                                FileUrl = GetStr(r5, cUrl),
                                CommentFile = GetStr(r5, cComment),
                                DateUpload = r5.IsDBNull(cDate) ? (DateTime?)null : r5.GetDateTime(cDate),
                                CodeOppor = GetStr(r5, cCodeOppor),
                                FileTitle = GetStr(r5, cTitle),
                                RelativePath = GetStr(r5, cRelativePath),
                                ArchiveType = GetStr(r5, cArchiveType)
                            });
                        }
                    }
                }

                // ---------- 6) HISTORILA DE CAMBIOS ----------
                await using (var cmdHistory = new SqlCommand("SP_CHANGE_HISTORY", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmdHistory.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdHistory.Parameters.Add("@ENTITY_ID", SqlDbType.BigInt).Value = LinkToken;

                    await using var r6 = await cmdHistory.ExecuteReaderAsync(ct);
                    if (r6.HasRows)
                    {

                        int cHistory = Ord(r6, "HISTORY");
                        int cUsersChange = Ord(r6, "USERS_CHANGE");
                        int cDateChange = Ord(r6, "CHANGED_AT");

                        while (await r6.ReadAsync(ct))
                        {
                            o.HistoryChanges.Add(new HistoryOpportunityChanges
                            {

                                History = r6.IsDBNull(cHistory) ? null : r6.GetString(cHistory),
                                UsersName = r6.IsDBNull(cUsersChange) ? null : r6.GetString(cUsersChange),
                                DateChange = r6.IsDBNull(cDateChange) ? (DateTime?)null : r6.GetDateTime(cDateChange)
                            });
                        }
                    }
                }

                return o;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el detalle de la oportunidad.", ex.Message);
            }
        }

        public async Task<Opportunities> GetClientsByIdAsync(string LinkToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_OPPORTUNITY_CLIENTS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_ID", LinkToken);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(0);
                    return new Opportunities
                    {
                        LinkToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        ClientsId = reader.GetInt64(2),
                        ReasonRejection = reader.IsDBNull(3) ? null : reader.GetString(3)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener al cliente por ID.", ex.Message);
            }
        }

        public async Task<Opportunities?> GetStateByIdAsync(string linkToken)
            {
                try
                {
                    using var cn = _connectionFactory.CreateConnection();
                    await cn.OpenAsync();

                    using var cmd = new SqlCommand("SP_WS_OPPORTUNITY_STATE_BY_ID", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.Add("@OPPOR_ID", SqlDbType.VarChar, 200).Value = linkToken;

                    using var reader = await cmd.ExecuteReaderAsync();

                    Opportunities? result = null;

                    // Resultset 1: cabecera
                    if (await reader.ReadAsync())
                    {
                        var opporId = reader.GetInt64(0);

                        result = new Opportunities
                        {
                            LinkToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                            BusinessId = reader.GetInt64(1),
                            StateOpporId = reader.IsDBNull(2) ? null : reader.GetInt64(2),
                            ReasonRejectionId = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                            ReasonRejection = reader.IsDBNull(4) ? null : reader.GetString(4),

                            CurrencyId = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                            WonComment = reader.IsDBNull(6) ? null : reader.GetString(6),

                            ViabilityScore = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            Compliance = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                            PartialCompliance = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                            NonCompliance = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),

                            Authority = reader.IsDBNull(11) ? null : reader.GetInt32(11),
                            AuthorityDesc = reader.IsDBNull(12) ? null : reader.GetString(12),

                            Budget = reader.IsDBNull(13) ? null : reader.GetInt32(13),
                            BudgetDesc = reader.IsDBNull(14) ? null : reader.GetString(14),

                            Need = reader.IsDBNull(15) ? null : reader.GetInt32(15),
                            NeedDesc = reader.IsDBNull(16) ? null : reader.GetString(16),

                            Term = reader.IsDBNull(17) ? null : reader.GetInt32(17),
                            TermDesc = reader.IsDBNull(18) ? null : reader.GetString(18),

                            CompanyExperience = reader.IsDBNull(19) ? null : reader.GetInt32(19),
                            CompanyExperienceDesc = reader.IsDBNull(20) ? null : reader.GetString(20),

                            WorkerExperience = reader.IsDBNull(21) ? null : reader.GetInt32(21),
                            WorkerExperienceDesc = reader.IsDBNull(22) ? null : reader.GetString(22),

                            StaffExperience = reader.IsDBNull(23) ? null : reader.GetInt32(23),
                            StaffExperienceDesc = reader.IsDBNull(24) ? null : reader.GetString(24),

                            Ability = reader.IsDBNull(25) ? null : reader.GetInt32(25),
                            AbilityDesc = reader.IsDBNull(26) ? null : reader.GetString(26),

                            Shedule = reader.IsDBNull(27) ? null : reader.GetInt32(27),
                            SheduleDesc = reader.IsDBNull(28) ? null : reader.GetString(28),

                            ContractMethod = reader.IsDBNull(29) ? null : reader.GetInt32(29),
                            ContractMethodDesc = reader.IsDBNull(30) ? null : reader.GetString(30),

                            RequiresIsos = reader.IsDBNull(31) ? null : reader.GetInt32(31),
                            RequiresIsosDesc = reader.IsDBNull(32) ? null : reader.GetString(32),

                            ProposalComment = reader.IsDBNull(33) ? null : reader.GetString(33),
                            DateFinish = reader.IsDBNull(34) ? null : reader.GetDateTime(34),
                            DateRegister = reader.IsDBNull(35) ? null : reader.GetDateTime(35),
                            IsHiring = reader.IsDBNull(36) ? null : reader.GetBoolean(36),

                            BrandAproach = reader.IsDBNull(37) ? null : reader.GetInt32(37),
                            BrandAproachDesc = reader.IsDBNull(38) ? null : reader.GetString(38),
                            TechnicalChanges = reader.IsDBNull(39) ? null : reader.GetInt32(39),
                            TechnicalChangesDesc = reader.IsDBNull(40) ? null : reader.GetString(40),
                            IsReEvaluation = reader.IsDBNull(41) ? null : reader.GetBoolean(41),
                            OpporNumber = reader.GetString(42),
                            StateOpporGenId = reader.GetInt64(43),


                            Deliverables = new List<DeliverablesOppor>(),
                            DeliverablesHiring = new List<DeliverablesOppor>(),
                            Observations = new List<ObservationOppor>(),
                        };
                    }

                    if (result == null) return null;

                // Resultset 2: deliverables normales
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Deliverables!.Add(new DeliverablesOppor
                        {
                            DeliverablesId = reader.GetInt64(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Comment = reader.IsDBNull(2) ? null : reader.GetString(2),
                            DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            State = reader.IsDBNull(4) ? null : reader.GetString(4),
                            RequestId = reader.IsDBNull(5) ? null : reader.GetInt64(5)
                        });
                    }
                }

                if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                        long? tasksId = reader.IsDBNull(4) ? (long?)null : reader.GetInt64(4);
                        result.DeliverablesHiring!.Add(new DeliverablesOppor
                        {
                            DeliverablesId = reader.GetInt64(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Comment = reader.IsDBNull(2) ? null : reader.GetString(2),
                            DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            TasksId = tasksId,
                            TaskToken = _linkTokenService.Issue("tasks", Convert.ToInt64(tasksId), TimeSpan.FromHours(1)),
                            State = reader.IsDBNull(5) ? null : reader.GetString(5),
                            TaskStateDesc = reader.IsDBNull(5) ? null : reader.GetString(5),
                            TaskStateColor = reader.IsDBNull(6) ? null : reader.GetString(6),
                            NumPercPro = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            RequestId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                            TypeDeliverable = reader.IsDBNull(9) ? null : reader.GetInt32(9)

                        });
                        }
                    }
                    
                if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Observations!.Add(new ObservationOppor
                            {
                                ObsId = reader.GetInt64(0),
                                OpporId = reader.GetInt64(1),
                                ObsSeverity = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                                ObsComment = reader.IsDBNull(3) ? null : reader.GetString(3),
                                DueDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                DueSetBy = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                                ObsSeverityDesc = reader.IsDBNull(6) ? null : reader.GetString(6),
                                ObsColor = reader.IsDBNull(7) ? null : reader.GetString(7),
                            });
                        }
                    }

                    return result;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al obtener oportunidad por ID.", ex.Message);
                }
            }

        public async Task<bool> UpdateClientsAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPORTUNITY_CLIENTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@REASON_REJECTION", (object?)(string.IsNullOrWhiteSpace(entity.ReasonRejection) ? null : entity.ReasonRejection.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar al cliente en base de datos.", ex.Message);
            }
        }
        public async Task<bool> UpdateStateAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var tx = cn.BeginTransaction();

                // 1) Resolver IDs de entregables custom
                await EnsureDeliverablesIdsAsync(entity, cn, tx);

                // 2) SP de actualización de estado
                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPOR_STATE", cn, tx)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_OPPORTUNITY_ID", (object?)entity.StateOpporId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@REASON_REJECTION_ID", (object?)entity.ReasonRejectionId ?? DBNull.Value);     
                cmd.Parameters.AddWithValue("@REASON_REJECTION", (object?)(string.IsNullOrWhiteSpace(entity.ReasonRejection) ? null : entity.ReasonRejection.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WON_COMMENT", (object?)entity.WonComment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PROPOSAL_PRESENTATED", (object?)entity.ProposalPresentated ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PROPOSAL_COMMENT", (object?)entity.ProposalComment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                // VIABILIDAD
                cmd.Parameters.AddWithValue("@VIABILITY_SCORE", (object?)entity.ViabilityScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@COMPLIANCE", (object?)entity.Compliance ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PARTIAL_COMPLIANCE", (object?)entity.PartialCompliance ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NON_COMPLIANCE", (object?)entity.NonCompliance ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AUTHORITY", (object?)entity.Authority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AUTHORITY_DESC", (object?)entity.AuthorityDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BUDGET", (object?)entity.Budget ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BUDGET_DESC", (object?)entity.BudgetDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NEED",    (object?)entity.Need ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NEED_DESC", (object?)entity.NeedDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TERM", (object?)entity.Term ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TERM_DESC", (object?)entity.TermDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CONTRACT_METHOD", (object?)entity.ContractMethod ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CONTRACT_METHOD_DESC", (object?)entity.ContractMethodDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@REQUIRES_ISOS", (object?)entity.RequiresIsos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@REQUIRES_ISOS_DESC", (object?)entity.RequiresIsosDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@COMPANY_EXPERIENCE", (object?)entity.CompanyExperience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@COMPANY_EXP_DESC", (object?)entity.CompanyExperienceDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WORK_EXPERIENCE", (object?)entity.WorkerExperience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WORK_EXP_DESC", (object?)entity.WorkerExperienceDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STAFF_EXPERIENCE", (object?)entity.StaffExperience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STAFF_EXP_DESC", (object?)entity.StaffExperienceDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ABILITY", (object?)entity.Ability ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ABILITY_DESC", (object?)entity.AbilityDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SCHEDULE", (object?)entity.Shedule ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SCHEDULE_DESC", (object?)entity.SheduleDesc ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@BRAND_APPROACH", (object?)entity.BrandAproach ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BRAND_APPROACH_DESC", (object?)entity.BrandAproachDesc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TECHNICAL_CHANGES", (object?)entity.TechnicalChanges ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TECHNICAL_CHANGES_DESC", (object?)entity.TechnicalChangesDesc ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@MIN_SCORE", (object?)entity.MinScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MAX_SCORE", (object?)entity.MaxScore ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@STATE_NEGOTATION_OUTCOME_ID", (object?)entity.NegotationOutcomeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NEGOTATION_REASON", (object?)entity.ReasonObsClients ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TYPE_OBS_CLIENTS", (object?)entity.TypeObsClientsId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TYPE_OBS_ECONOMIC", (object?)entity.TypeObsEconomic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IS_RE_EVALUATION", (object?)entity.IsReEvaluation ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FILE_TITLE", (object?)entity.FileTitle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILE_URL", (object?)entity.FileUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RELATIVE_PATH", (object?)entity.RelativePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ARCHIVE_TYPE", (object?)entity.ArchiveType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SELECT_QUOTATION_VER_ID", (object?)entity.QuotationVerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("CALL_DATE",(object?)entity.CallDate ??  DBNull.Value);
                cmd.Parameters.AddWithValue("@FOLLOWUP_ENABLED", (object?)entity.FollowupEnabled ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PM_CONDITION_ID", entity.PmConditionId);

                // TVP DE DELIVERABLES PREVENTA
                var tvp = BuildDeliverablesTvp(entity.Deliverables ?? new List<DeliverablesOppor>());
                var pDeliverables = cmd.Parameters.Add("@DELIVERABLES", SqlDbType.Structured);
                pDeliverables.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                pDeliverables.Value = tvp;

                // TVP DELIVERABLES CONTRATACIONES
                var tvpHiring = BuildDeliverablesTvp(entity.DeliverablesHiring ?? new List<DeliverablesOppor>());
                var pDeliverablesHiring = cmd.Parameters.Add("@DELIVERABLES_HIRING", SqlDbType.Structured);
                pDeliverablesHiring.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                pDeliverablesHiring.Value = tvpHiring;

                // TVP DELIVERABLES OBSERVACIONES
                var tvpObs = BuildDeliverablesObsTvp(entity.Observations ?? new List<ObservationOppor>());
                var pDeliverablesObs = cmd.Parameters.Add("@DELIVERABLES_OBS", SqlDbType.Structured);
                pDeliverablesObs.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                pDeliverablesObs.Value = tvpObs;

                var rows = await cmd.ExecuteNonQueryAsync();

                tx.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar al cliente en base de datos.", ex.Message);
            }
        }
        public async Task<bool> UploadNewVerAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var tx = cn.BeginTransaction();
                // 1) SP de actualización de estado
                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPOR_NEW_VER", cn, tx)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@OPPOR_ID", SqlDbType.BigInt).Value = entity.LinkToken;
                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = entity.BusinessId;
                cmd.Parameters.Add("@PROPOSAL_COMMENT", SqlDbType.VarChar, -1).Value =
                    (object?)entity.ProposalComment ?? DBNull.Value;
                cmd.Parameters.Add("@UPDATE_USER", SqlDbType.Int).Value = entity.UsersBy;

                cmd.Parameters.Add("@FILE_TITLE", SqlDbType.VarChar, -1).Value =
                    (object?)entity.FileTitle ?? DBNull.Value;
                cmd.Parameters.Add("@FILE_URL", SqlDbType.VarChar, -1).Value =
                    (object?)entity.FileUrl ?? DBNull.Value;
                cmd.Parameters.Add("@RELATIVE_PATH", SqlDbType.VarChar, -1).Value =
                    (object?)entity.RelativePath ?? DBNull.Value;
                cmd.Parameters.Add("@ARCHIVE_TYPE", SqlDbType.VarChar, -1).Value =
                    (object?)entity.ArchiveType ?? DBNull.Value;

                string? message = null;
                int status = 0;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        message = reader["message"] as string;
                        status = Convert.ToInt32(reader["status"]);
                    }
                }

                if (status != 1)
                {
                    tx.Rollback();
                    throw new DatabaseException("SP devolvió error.", message ?? "Sin mensaje");
                }

                tx.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la oportunidad en base de datos.", ex.Message);
            }
        }
        public async Task<PagedSelect<OptionItem>> GetForQuoVerSelectAsync(long businessId, string linkToken, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_VERION_NO_QUOTATION_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", linkToken);
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
                throw new DatabaseException("Error al obtener el versionamiento de cotizacion para el selector.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForFlowTypeSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_FLOW_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener tipo de atencionen en el selector.", ex.Message);
            }
        }


        //*=============================DELIVERABLES=================================*//

        public async Task<PagedSelect<OptionItem>> GetForSelectDeliverablesAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_ST_OPPOR_DELIVERABLES", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener los cliente para el selector.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectDeliverablesHiringAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_ST_OPPOR_DELIVERABLES_HIRING", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener los cliente para el selector.", ex.Message);
            }
        }

        private async Task EnsureDeliverablesIdsAsync(Opportunities entity, SqlConnection cn, SqlTransaction tx)
        {
            if (entity.Deliverables == null || !entity.Deliverables.Any())
                return;

            var pending = entity.Deliverables
                .Where(d =>
                    (d.DeliverablesId <= 0 || d.DeliverablesId == null) &&
                    !string.IsNullOrWhiteSpace(d.Name))
                .ToList();

            foreach (var item in pending)
            {
                var trimmedName = item.Name!.Trim();

                if (string.IsNullOrWhiteSpace(trimmedName))
                    continue;

                var newId = await GetOrCreateDeliverableIdAsync(
                    cn,
                    tx,
                    entity.BusinessId,
                    trimmedName,
                    entity.UsersBy
                );

                item.DeliverablesId = newId;
            }
        }
        private async Task<int> GetOrCreateDeliverableIdAsync(SqlConnection cn,SqlTransaction tx, long businessId, string name,long userId)
        {
            using var cmd = new SqlCommand("SP_WS_UPSERT_DELIVERABLE", cn, tx)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@NAME", name);
            cmd.Parameters.AddWithValue("@CREATE_USER", userId);

            var outId = new SqlParameter("@DELIVERABLES_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outId);

            await cmd.ExecuteNonQueryAsync();

            return (int)outId.Value;
        }
        private static DataTable BuildDeliverablesTvp(List<DeliverablesOppor>? deliverables)
        {
            var table = new DataTable();
            table.Columns.Add("DELIVERABLES_ID", typeof(int));
            table.Columns.Add("COMMENT", typeof(string));
            table.Columns.Add("DUE_DATE", typeof(DateTime));

            foreach (var d in deliverables)
            {
                if (d.DeliverablesId == null || d.DeliverablesId <= 0)
                    continue;

                table.Rows.Add(
                    d.DeliverablesId,
                    d.Comment ?? string.Empty,
                    (object?)d.DueDate ?? DBNull.Value
                );
            }

            return table;
        }
        private static DataTable BuildDeliverablesObsTvp(List<ObservationOppor>? deliverables)
        {
            var table = new DataTable();
            table.Columns.Add("DELIVERABLES_ID", typeof(int));
            table.Columns.Add("COMMENT", typeof(string));
            table.Columns.Add("DUE_DATE", typeof(DateTime));

            foreach (var d in deliverables)
            {
                if (d.ObsId == null || d.ObsId <= 0)
                    continue;

                table.Rows.Add(
                    d.ObsId,
                    d.ObsComment ?? string.Empty,
                    (object?)d.DueDate ?? DBNull.Value
                );
            }

            return table;
        }
        private static DataTable BuildDeliverablesHiringFileTvp(List<OpportFiletracking>? deliverables)
        {
            var table = new DataTable();
            table.Columns.Add("FILE_TRACKING_ID", typeof(int));
            table.Columns.Add("FILE_TITLE", typeof(string));
            table.Columns.Add("FILE_URL", typeof(string));
            table.Columns.Add("RELATIVE_PATH", typeof(string));
            table.Columns.Add("ARCHIVE_TYPE", typeof(string));

            foreach (var d in deliverables)
            {
                if (d.FileUrl == null)
                    continue;

                table.Rows.Add(
                    d.LinkToken,
                    d.FileTitle ?? string.Empty,
                    d.FileUrl ?? string.Empty,
                    d.RelativePath ?? string.Empty,
                    d.ArchiveType ?? string.Empty
                );
            }

            return table;
        }
        public async Task<GlobalResponse> UpdateDeliverablesOnlyAsync(Opportunities entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var tx = cn.BeginTransaction();

                try
                {
                    await EnsureDeliverablesIdsAsync(entity, cn, tx);

                    using var cmd = new SqlCommand("SP_WS_UPDATE_OPPOR_DELIVERABLES_ONLY", cn, tx)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@OPPOR_ID", entity.LinkToken);
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                    var tvp = BuildDeliverablesTvp(entity.Deliverables);

                    var pDeliverables = cmd.Parameters.Add("@DELIVERABLES", SqlDbType.Structured);
                    pDeliverables.TypeName = "dbo.TT_OPPOR_DELIVERABLE";
                    pDeliverables.Value = tvp;

                    using var reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        var status = reader["status"] is int s ? s : Convert.ToInt32(reader["status"]);
                        var message = reader["message"]?.ToString() ?? string.Empty;

                        reader.Close();

                        tx.Commit();

                        return new GlobalResponse
                        {
                            Status = status,
                            Message = message
                        };
                    }

                    tx.Rollback();
                    return new GlobalResponse
                    {
                        Status = 0,
                        Message = "El procedimiento no devolvió resultados."
                    };
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar los entregables en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar los entregables.", ex.Message);
            }
        }


    }
}
