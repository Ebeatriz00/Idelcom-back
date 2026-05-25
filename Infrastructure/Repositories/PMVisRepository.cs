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
    public class PMVisRepository : IPMVisRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public PMVisRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? pmVisId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM PM_VIS WHERE DESCRIPTION = @DESCRIPTION AND BUSINESS_ID = @BID");

                if (pmVisId.HasValue)
                {
                    query.Append(" AND PM_VIS_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@DESCRIPTION", description);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (pmVisId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", pmVisId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la visita.", ex.Message);
            }
        }

        public async Task AddAsync(PMVis entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PM_VIS", connection)
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
                throw new DatabaseException("Error al registrar la visita en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<PMVis>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<PMVis>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_PM_VIS", cn)
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
                    list.Add(new PMVis
                    {
                        PMVisId = reader.GetInt64(0),
                        Description = reader.GetString(1),
                        Status = reader.GetString(2),
                        PMVisCount = reader.GetInt32(3)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<PMVis>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de visitas paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PM_VIS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
                throw new DatabaseException("Error al obtener las visitas para el selector.", ex.Message);
            }
        }

        public async Task<PMVis> GetByIdAsync(long pmVisId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_PM_VIS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PM_VIS_ID", pmVisId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new PMVis
                    {
                        PMVisId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la visita por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(PMVis entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PM_VIS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PM_VIS_ID", entity.PMVisId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la visita en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long pmVisId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PM_VIS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PM_VIS_ID", pmVisId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la visita.", ex.Message);
            }
        }
    }
}
