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
    public class ParentModulesRepository : IParentModulesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ParentModulesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> ExistsAsync(string title, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var query = new StringBuilder(@"
                                SELECT COUNT(*) 
                                FROM PARENT_MODULES 
                                WHERE UPPER(TITLE) = UPPER(@NAME) 
                                  AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    query.Append(" AND PARENT_MODULES_ID <> @ID");

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@NAME", title.Trim());
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del módulo padre.", ex.Message);
            }
        }

        public async Task AddAsync(ParentModules entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PARENT_MODULES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CODE", entity.Code);
                cmd.Parameters.AddWithValue("@TITLE", entity.Title);
                cmd.Parameters.AddWithValue("@STICKY_BOTTOM", entity.StickyBottom);
                cmd.Parameters.AddWithValue("@ICON_KEY", entity.IconKey ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ORDER_NO", entity.OrderNo);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar módulo padre en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar módulo padre.", ex.Message);
            }
        }

        public async Task<PagedResult<ParentModules>> GetAllAsync(int businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<ParentModules>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_PARENT_MODULES", cn)
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
                    list.Add(new ParentModules
                    {
                        ParentModulesId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Code = reader.GetString(2),
                        Title = reader.GetString(3),
                        StickyBottom = reader.GetString(4),
                        OrderNo = reader.GetInt32(5),
                        Status = reader.GetString(6),
                        ParentCount = reader.GetInt32(7)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ParentModules>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de módulos.", ex.Message);
            }

        }

        public async Task<ParentModules> GetByIdAsync(long parentModulesId)
        {
            try
            {
                ParentModules? parentModules = null;
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_PARENT_MODULES_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", parentModulesId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    parentModules = new ParentModules
                    {
                        ParentModulesId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Code = reader.GetString(2),
                        Title = reader.GetString(3),
                        StickyBottom = reader.GetString(4),
                        OrderNo = reader.GetInt32(5)
                    };
                }
                return parentModules;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener módulo padre por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(ParentModules parentModules)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PARENT_MODULES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", parentModules.BusinessId);
                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", parentModules.ParentModulesId);
                cmd.Parameters.AddWithValue("@CODE", parentModules.Code);
                cmd.Parameters.AddWithValue("@TITLE", parentModules.Title);
                cmd.Parameters.AddWithValue("@STICKY_BOTTOM", parentModules.StickyBottom);
                cmd.Parameters.AddWithValue("@ICON_KEY", parentModules.IconKey ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ORDER_NO", parentModules.OrderNo);
                cmd.Parameters.AddWithValue("@UPDATE_USER", parentModules.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();


            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar módulo padre.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long parentModulesId, string status, int updatedBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PARENT_MODULES_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", parentModulesId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", updatedBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar estado del módulo padre.", ex.Message);
            }
        }
    }
}
