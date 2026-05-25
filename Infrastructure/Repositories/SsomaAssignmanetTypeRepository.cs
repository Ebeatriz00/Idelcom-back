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
    public class SsomaAssignmanetTypeRepository : ISsomaAssignmanetTypeRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public SsomaAssignmanetTypeRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> ExistsAsync(string description, long businessId, long? ssomaAssignmentTypeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var query = new StringBuilder(@"
                                                SELECT COUNT(*)
                                                FROM SSOMA_ASSIGNMENT_TYPE
                                                WHERE SSOMA_ASSIGNMENT_TYPE_DESC LIKE '%' + @SSOMA_ASSIGNMENT_TYPE_DESC + '%'
                                                  AND BUSINESS_ID = @BID");

                if (ssomaAssignmentTypeId.HasValue)
                {
                    query.Append(" AND SSOMA_ASSIGNMENT_TYPE_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);

                cmd.Parameters.Add("@SSOMA_ASSIGNMENT_TYPE_DESC", SqlDbType.VarChar, 255).Value = description;
                cmd.Parameters.Add("@BID", SqlDbType.BigInt).Value = businessId;

                if (ssomaAssignmentTypeId.HasValue)
                {
                    cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = ssomaAssignmentTypeId.Value;
                }

                await connection.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                var count = Convert.ToInt32(result);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del tipo de asignación SSOMA.", ex.Message);
            }
        }
        public async Task AddAsync(SsomaAssignmanetType entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_SSOMA_ASSIGNMANET_TYPE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.SsomaAssignamentName);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la visita en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<SsomaAssignmanetType>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<SsomaAssignmanetType>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_LIST_SSOMA_ASSIGNMENT_TYPE", cn)
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
                    list.Add(new SsomaAssignmanetType
                    {
                        SsomaAssignamentTypeId = reader.GetInt32(0),
                        SsomaAssignamentName = reader.GetString(1),
                        Status = reader.GetString(2)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<SsomaAssignmanetType>
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

                using var cmd = new SqlCommand("SP_WS_SSOMA_ASSIGNMENT_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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

        public async Task<SsomaAssignmanetType> GetByIdAsync(long SsomaAssignmanetTypeId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_SSOMA_ASSIGNMENT_TYPE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SSOMA_ASSIGNMENT_TYPE_ID", SsomaAssignmanetTypeId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SsomaAssignmanetType
                    {
                        SsomaAssignamentTypeId = reader.GetInt32(0),
                        BusinessId = reader.GetInt64(1),
                        SsomaAssignamentName = reader.GetString(2)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la visita por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(SsomaAssignmanetType entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_SSOMA_ASSIGNMANET_TYPE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SSOMA_ASSIGNMENT_TYPE_ID", entity.SsomaAssignamentTypeId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@SSOMA_ASSIGNMENT_TYPE_DESC", entity.SsomaAssignamentName);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la visita en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long SsomaAssignmanetTypeId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_SSOMA_ASSIGNMENT_TYPE_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PM_VIS_ID", SsomaAssignmanetTypeId);
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
