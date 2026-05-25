using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
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
    public class PeriodsRepository : IPeriodsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public PeriodsRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long exercisesId, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM PERIODS WHERE DESCRIPTION = @DESCRIPTION AND EXERCISES_ID = @EID AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND PERIODS_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@DESCRIPTION", description);
                cmd.Parameters.AddWithValue("@EID", exercisesId);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del período.", ex.Message);
            }
        }

        public async Task AddAsync(Periods entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PERIODS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@EXERCISES_ID", entity.ExercisesId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", entity.EndDate);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);
                cmd.Parameters.AddWithValue("@IND_BLOCK", entity.IndBlock);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el período en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el período.", ex.Message);
            }
        }

        public async Task<PagedResult<Periods>> GetAllAsync(long businessId, long exercisesId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<Periods>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_PERIODS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@EXERCISES_ID", exercisesId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)usersBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new Periods
                    {
                        BusinessId = reader.GetInt64(0),
                        PeriodsId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                        EndDate = reader.GetDateTime(3),
                        Status = reader.GetString(4),
                        ExercisesDescription = reader.GetString(5),
                        ExercisesEndDate = reader.GetDateTime(6),
                        PeriodsCount = reader.GetInt32(7),
                        IndBlock = reader.GetBoolean(reader.GetOrdinal("IND_BLOCK"))
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Periods>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de períodos paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PERIODS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener los períodos para el selector.", ex.Message);
            }
        }

        public async Task<Periods> GetByIdAsync(long periodsId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_PERIODS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PERIODS_ID", periodsId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Periods
                    {
                        PeriodsId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                        EndDate = reader.GetDateTime(3),
                        ExercisesId = reader.GetInt64(4),
                        Status = reader.GetString(reader.GetOrdinal("STATUS")),
                        IndBlock = reader.GetBoolean(reader.GetOrdinal("IND_BLOCK"))
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el período por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Periods entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PERIODS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PERIODS_ID", entity.PeriodsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@EXERCISES_ID", entity.ExercisesId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", entity.EndDate);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el período en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long periodsId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PERIODS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PERIODS_ID", periodsId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del período.", ex.Message);
            }
        }

        public async Task<bool> PatchBlockStatusAsync(long periodsId, bool indBlock, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PERIODS_BLOCK", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PERIODS_ID", periodsId);
                cmd.Parameters.AddWithValue("@IND_BLOCK", indBlock);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de bloqueo del período.", ex.Message);
            }
        }

        public async Task<bool> HasActivePeriodsAsync(long exercisesId, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = @"
            SELECT COUNT(*) 
            FROM dbo.PERIODS 
            WHERE EXERCISES_ID = @EID 
            AND BUSINESS_ID = @BID 
            AND IND_BLOCK = '1'";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EID", exercisesId);
                cmd.Parameters.AddWithValue("@BID", businessId);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al verificar períodos activos.", ex.Message);
            }
        }




    }
}
