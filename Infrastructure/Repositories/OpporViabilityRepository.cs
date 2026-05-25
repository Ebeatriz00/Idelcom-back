using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OpporViabilityRepository : IOpporViabilityRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public OpporViabilityRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, string? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM OPPOR_VIABILITY WHERE OPPOR_DESC = @DESC AND BUSINESS_ID = @BID AND STATUS = '1'";

                if (!string.IsNullOrEmpty(excludeId))
                    query += " AND OPPOR_VIABILITY_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DESC", description);
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

        public async Task<PagedResult<OpporViability>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<OpporViability>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_OPPOR_VIABILITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)usersBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt64(0);

                    list.Add(new OpporViability
                    {
                        LinkToken = _linkTokenService.Issue("opporviability", reader.GetInt64(0), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        OpporNum = reader.GetString(2),
                        OpporDesc = reader.GetString(3),
                        ClientsId = reader.GetInt64(4),
                        ClientsName = reader.GetString(5),
                        GeneralStatesId = reader.GetInt64(6),
                        GeneralStatesName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        ColorState = reader.IsDBNull(8) ? "" : reader.GetString(8),

                        // VIABILITY
                        ViabilityScore = reader.GetInt32(9),
                        Compliance = reader.GetInt32(10),
                        PartialCompliance = reader.GetInt32(11),
                        NonCompliance = reader.GetInt32(12),
                        Authority = reader.GetInt32(13),
                        AuthorityDesc = reader.IsDBNull(14) ? null : reader.GetString(14),
                        Budget = reader.GetInt32(15),
                        BudgetDesc = reader.IsDBNull(16) ? null : reader.GetString(16),
                        Need = reader.GetInt32(17),
                        NeedDesc = reader.IsDBNull(18) ? null : reader.GetString(18),
                        Term = reader.GetInt32(19),
                        TermDesc = reader.IsDBNull(20) ? null : reader.GetString(20),
                        CompanyExperience = reader.GetInt32(21),
                        CompanyExperienceDesc = reader.IsDBNull(22) ? null : reader.GetString(22),
                        WorkerExperience = reader.GetInt32(23),
                        WorkerExperienceDesc = reader.IsDBNull(24) ? null : reader.GetString(24),
                        StaffExperience = reader.GetInt32(25),
                        StaffExperienceDesc = reader.IsDBNull(26) ? null : reader.GetString(26),
                        Ability = reader.GetInt32(27),
                        AbilityDesc = reader.IsDBNull(28) ? null : reader.GetString(28),
                        Schedule = reader.GetInt32(29),
                        ScheduleDesc = reader.IsDBNull(30) ? null : reader.GetString(30),

                        //*************************************************************************//
                        StateOpportunityId = reader.GetInt64(31),
                        OporStatesName = reader.GetString(32),
                        OporColorState = reader.GetString(33),
                        UsersBy = reader.GetInt64(34),
                        Status = reader.GetString(35),
                        IsHiring = reader.IsDBNull(36) ? null : reader.GetBoolean(36),
                        IsReEvaluation = reader.IsDBNull(37) ? null : reader.GetBoolean(37),
                        ContractMethod = reader.IsDBNull(38) ? null : reader.GetInt32(38),
                        ContractMethodDesc = reader.IsDBNull(39) ? null : reader.GetString(39),
                        RequiresIsos = reader.IsDBNull(40) ? null : reader.GetInt32(40),
                        RequiresIsosDesc = reader.IsDBNull(41) ? null : reader.GetString(41),
                        BrandAproach = reader.IsDBNull(42) ? null : reader.GetInt32(42),
                        BrandAproachDesc = reader.IsDBNull(43) ? null : reader.GetString(43),
                        TechnicalChanges = reader.IsDBNull(44) ? null : reader.GetInt32(44),
                        TechnicalChangesDesc = reader.IsDBNull(45) ? null : reader.GetString(45)

                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<OpporViability>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de oportunidades paginada.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_OPPOR_VIABILITY_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@OPPOR_VIABILITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la oportunidad.", ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> ProcessDecisionAsync(long opporId, long businessId, long usersBy, bool isApproved, string? rejectionReason)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_CONVERT_PRE_OPPORTUNITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@IS_APPROVED", isApproved);
                cmd.Parameters.AddWithValue("@REJECTION_REASON", (object?)rejectionReason ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var message = reader.GetString(reader.GetOrdinal("message"));
                    var status = reader.GetInt32(reader.GetOrdinal("status"));
                    return (status == 1, message);
                }

                return (false, "No se recibió respuesta del servidor.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al procesar la decisión de la oportunidad.", ex.Message);
            }
        }


    }
}
