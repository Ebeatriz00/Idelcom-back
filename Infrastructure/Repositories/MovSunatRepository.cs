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
    public class MovSunatRepository : IMovSunatRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public MovSunatRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM dbo.MOV_SUNAT WHERE DESCRIPTION = @DESCRIPTION AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND MOV_SUNAT_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@DESCRIPTION", description);
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
                throw new DatabaseException("Error al validar existencia del tipo SUNAT.", ex.Message);
            }
        }

        public async Task AddAsync(MovSunat entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_MOV_SUNAT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo SUNAT en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<MovSunat>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<MovSunat>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_MOV_SUNAT", cn)
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
                    list.Add(new MovSunat
                    {
                        MovSunatId = reader.GetInt64(0),
                        Description = reader.GetString(1),
                        Status = reader.GetString(2),
                        MovSunatCount = reader.GetInt32(3)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<MovSunat>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos SUNAT paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_MOV_SUNAT_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener los tipos SUNAT para el selector.", ex.Message);
            }
        }

        public async Task<MovSunat> GetByIdAsync(long movSunatId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_MOV_SUNAT_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOV_SUNAT_ID", movSunatId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new MovSunat
                    {
                        MovSunatId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo SUNAT por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(MovSunat entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_MOV_SUNAT", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOV_SUNAT_ID", entity.MovSunatId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo SUNAT en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long movSunatId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_MOV_SUNAT_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOV_SUNAT_ID", movSunatId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del tipo SUNAT.", ex.Message);
            }
        }
    }
}
