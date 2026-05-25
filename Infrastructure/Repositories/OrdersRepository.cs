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
    public class OrdersRepository : IOrdersRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public OrdersRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<PagedResult<Orders>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<Orders>();
                int total = 0;

                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_OPERATIONS", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(1);
                    list.Add(new Orders
                    {
                        OperationsId = reader.GetInt64(0),
                        OpporId = opporId,
                        OpporToken = _linkTokenService.Issue("opportunity", Convert.ToInt64(opporId), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(2),
                        OpporNum = reader.GetString(3),
                        OpporDesc = reader.GetString(4),
                        ClientsName = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Commercial = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Responsible = reader.IsDBNull(7) ? null : reader.GetString(7),
                        QualitySupervisor = reader.IsDBNull(8) ? null : reader.GetString(8),
                        ProjectManager = reader.IsDBNull(9) ? null : reader.GetString(9),
                        Ssoma = reader.IsDBNull(10) ? null : reader.GetString(10),
                        SsomaIds = reader.IsDBNull(11) ? null : reader.GetString(11),
                        RequeredSsoma = reader.IsDBNull(12) ? null : reader.GetBoolean(12),
                        PlannedStartDate = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                        PlannedEndDate = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                        TypeOppor = reader.IsDBNull(16) ? null : Convert.ToString(reader.GetValue(16)),
                        ParentOpportunityId = reader.IsDBNull(17) ? null : Convert.ToInt64(reader.GetValue(17)),
                        AdditionalSequence = reader.IsDBNull(18) ? null : Convert.ToInt32(reader.GetValue(18)),
                        StateColor = reader.IsDBNull(19) ? null : reader.GetString(19),
                    });
                }

                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Orders>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de cliente paginada.", ex.Message);
            }
        }


        public async Task<bool> AddSsomaAsync(IEnumerable<Orders> entities)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                foreach (var entity in entities)
                {
                    using var cmd = new SqlCommand("SP_WS_REGISTER_SSOMA", connection, tx)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@OPERATIONS_ID", entity.OperationsId);
                    cmd.Parameters.Add("@WORKER_ID", SqlDbType.BigInt).Value = entity.WorkerId;
                    cmd.Parameters.AddWithValue("@REQUERED_SSOMA", entity.RequeredSsoma);
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new DatabaseException("Error al registrar la asignación de SSOMA.", ex.Message);
            }
        }

        public async Task<bool> ExistsSsomaAsync(long operationsId, long workerId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                string query = "SELECT COUNT(*) FROM dbo.OPERATIONS_SSOMA WHERE OPERATIONS_ID = @OID AND WORKER_ID = @WID AND STATUS = '1'";

                using var cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@OID", operationsId);
                cmd.Parameters.AddWithValue("@WID", workerId);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new Exceptions.DatabaseException("Error al validar existencia de la asignación del especialista SSOMA.", ex.Message);
            }
        }

        public async Task<bool> AddQualitySupervisorAsync(Orders entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_REGISTER_QUALITY_SUPERVISOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPERATIONS_ID", entity.OperationsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el Supervisor de Calidad en la operación.", ex.Message);
            }
        }

        public async Task<bool> AddProjectManagerAsync(Orders entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_REGISTER_PROJECT_MANAGER", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPERATIONS_ID", entity.OperationsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)entity.WorkerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el Gerente de Proyecto en la operación.", ex.Message);
            }
        }


    }
}
