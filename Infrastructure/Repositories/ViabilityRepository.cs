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
    public class ViabilityRepository : IViabilityRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public ViabilityRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string viability, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM VIABILITY WHERE OPPOR_ID = @OPPOR_ID AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND VIABILITY_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OPPOR_ID", viability);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la viabilidad.", ex.Message);
            }
        }

       
        public async Task<PagedResult<Viability>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<Viability>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_VIABILITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var viabilityId = reader.GetInt64(0);
                    list.Add(new Viability
                    {
                        linkToken = _linkTokenService.Issue("viability", viabilityId, TimeSpan.FromHours(1)),
                        ViabilityDecision = reader.GetInt32(1),
                        OpporToken = reader.IsDBNull(2) ? null : reader.GetInt64(2).ToString(), 
                        Authority = reader.GetInt32(3),
                        Budget = reader.GetInt32(4),
                        Need = reader.GetInt32(5),
                        Term = reader.GetInt32(6),
                        WorkExperience = reader.GetInt32(7),
                        StaffExperience = reader.GetInt32(8),
                        CompanyExperience = reader.GetInt32(9),
                        Ability = reader.GetInt32(10),
                        Schedule = reader.GetInt32(11),
                        AuthorityDesc = reader.IsDBNull(12) ? null : reader.GetString(12),
                        BudgetDesc = reader.IsDBNull(13) ? null : reader.GetString(13),
                        NeedDesc = reader.IsDBNull(14) ? null : reader.GetString(14),
                        TermDesc = reader.IsDBNull(15) ? null : reader.GetString(15),
                        WorkExpDesc = reader.IsDBNull(16) ? null : reader.GetString(16),
                        StaffExpDesc = reader.IsDBNull(17) ? null : reader.GetString(17),
                        CompanyExpDesc = reader.IsDBNull(18) ? null : reader.GetString(18),
                        AbilityDesc = reader.IsDBNull(19) ? null : reader.GetString(19),
                        ScheduleDesc = reader.IsDBNull(20) ? null : reader.GetString(20),
                        Status = reader.GetString(21)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Viability>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de viabilidad paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_VIABILITY_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener la viabilidad para el selector.", ex.Message);
            }
        }

        public async Task<Viability> GetByIdAsync(string LinkToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_VIABILITY_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@VIABILITY_ID", LinkToken);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var id = reader.GetInt64(0);
                    return new Viability
                    {
                        linkToken = _linkTokenService.Issue("viability", id, TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        Approved = reader.GetInt32(2),
                        ViabilityDecision = reader.GetInt32(3),
                        OpporToken = reader.IsDBNull(4) ? null : reader.GetInt64(4).ToString(), 
                        PartialCompliance = reader.GetInt32(5),
                        Compliance = reader.GetInt32(6),
                        NonCompliance = reader.GetInt32(7),
                        Authority = reader.GetInt32(8),
                        Budget = reader.GetInt32(9),
                        Need = reader.GetInt32(10),
                        Term = reader.GetInt32(11),
                        WorkExperience = reader.GetInt32(12),
                        StaffExperience = reader.GetInt32(13),
                        CompanyExperience = reader.GetInt32(14),
                        Ability = reader.GetInt32(15),
                        Schedule = reader.GetInt32(16),
                        AuthorityDesc = reader.IsDBNull(17) ? null : reader.GetString(17),
                        BudgetDesc = reader.IsDBNull(18) ? null : reader.GetString(18),
                        NeedDesc = reader.IsDBNull(19) ? null : reader.GetString(19),
                        TermDesc = reader.IsDBNull(20) ? null : reader.GetString(20),
                        WorkExpDesc = reader.IsDBNull(21) ? null : reader.GetString(21),
                        StaffExpDesc = reader.IsDBNull(22) ? null : reader.GetString(22),
                        CompanyExpDesc = reader.IsDBNull(23) ? null : reader.GetString(23),
                        AbilityDesc = reader.IsDBNull(24) ? null : reader.GetString(24),
                        ScheduleDesc = reader.IsDBNull(25) ? null : reader.GetString(25)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la viabilidad por ID.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_VIABILITY_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@VIABILITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la viabilidad.", ex.Message);
            }
        }

       
    }
}
