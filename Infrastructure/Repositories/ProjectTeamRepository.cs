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
    public class ProjectTeamRepository : IProjectTeamRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;
        public ProjectTeamRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(long businessId, long projectId, long workerId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var query = new StringBuilder("SELECT COUNT(*) FROM dbo.PROJECT_TEAM WHERE BUSINESS_ID = @BID AND PROJECT_ID = @PID AND WORKER_ID = @WID");

                if (excludeId.HasValue)
                    query.Append(" AND PROJECT_TEAM_ID <> @ID");

                using var cmd = new SqlCommand(query.ToString(), connection);

                cmd.Parameters.AddWithValue("@BID", businessId);
                cmd.Parameters.AddWithValue("@PID", projectId);
                cmd.Parameters.AddWithValue("@WID", workerId);

                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new Exceptions.DatabaseException("Error al validar existencia de la asignación del equipo de proyecto.", ex.Message);
            }
        }




        public async Task AddAsync(IEnumerable<ProjectTeam> entities)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                foreach (var entity in entities)
                {
                    using var cmd = new SqlCommand("SP_WS_REGISTER_PROJECT_TEAM", connection, tx)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.Add("@PROJECT_ID", SqlDbType.BigInt).Value = entity.ProjectId;
                    cmd.Parameters.Add("@WORKER_ID", SqlDbType.BigInt).Value = entity.WorkerId;
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new DatabaseException("Error al registrar múltiples permisos.", ex.Message);
            }
        }

        public async Task<PagedResult<ProjectTeam>> GetAllAsync(long businessId, long? projectId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<ProjectTeam>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PROJECT_TEAM_LIST", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PROJECT_ID", (object?)projectId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new ProjectTeam
                    {
                        ProjectTeamId = reader.GetInt64(0),
                        ProjectId = reader.GetInt64(1),
                        JobTitle = reader.GetString(2),
                        WorkerName = reader.GetString(3),

                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ProjectTeam>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de los permisod de perfiles paginada.", ex.Message);
            }
        }



        public async Task<bool> DeleteAsync(long businessId, long projectTeamId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PROJECT_TEAM_DELETE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PROJECT_TEAM_ID", projectTeamId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar la tarea de una oportunidad.", ex.Message);
            }
        }
    }
}