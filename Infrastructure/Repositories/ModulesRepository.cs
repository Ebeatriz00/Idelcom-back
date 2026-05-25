using Azure;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ModulesRepository : IModulesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ModulesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Modules entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_MODULES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", entity.ParentModulesId);
                cmd.Parameters.AddWithValue("@PARENT_ID", entity.ParentId);
                cmd.Parameters.AddWithValue("@CODE", entity.Code);
                cmd.Parameters.AddWithValue("@LABEL", entity.Label);
                cmd.Parameters.AddWithValue("@MODULES_DESCRIPTION", entity.ModulesDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ICON", entity.Icon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PATH", entity.Path ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ORDER_NO", entity.OrderNo);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar módulo en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar módulo.", ex.Message);
            }
        }

        public async Task<List<Modules>> GetAllAsync(long businessId, long parentModulesId, string? search, long? usersId)
        {
            try
            {
                var list = new List<Modules>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_MODULES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", parentModulesId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@USERS_ID",usersId.HasValue ? usersId.Value : DBNull.Value
);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Modules
                    {
                        ModulesId = reader.GetInt64(0),
                        ParentId= reader.IsDBNull(1) ? null : reader.GetInt64(1),
                        Code = reader.GetString(2),
                        Label = reader.GetString(3),
                        Path = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Icon = reader.IsDBNull(5) ? null : reader.GetString(5),
                        OrderNo = reader.GetInt32(6),
                        Status = reader.GetString(7)
                    });
                }
                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de módulos.", ex.Message);
            }
        }
        public async Task<PagedSelect<OptionItem>> GetModulesForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_MODULES_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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

            // N+1: si vino uno extra, hay más
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
        
        public async Task<Modules> GetByIdAsync(long modulesId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_MODULES_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MODULES_ID", modulesId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Modules
                    {
                        ModulesId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        ParentModulesId = reader.GetInt64(2),
                        ParentId = reader.GetInt64(3),
                        Code = reader.GetString(4),
                        Label = reader.GetString(5),
                        ModulesDescription = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Icon = reader.IsDBNull(7) ? null : reader.GetString(7),
                        Path = reader.IsDBNull(8) ? null : reader.GetString(8),
                        OrderNo = reader.GetInt32(9)

                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener módulo por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Modules modules)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_MODULES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", modules.BusinessId);
                cmd.Parameters.AddWithValue("@MODULES_ID", modules.ModulesId);
                cmd.Parameters.AddWithValue("@PARENT_MODULES_ID", modules.ParentModulesId);
                cmd.Parameters.AddWithValue("@PARENT_ID", modules.ParentId);
                cmd.Parameters.AddWithValue("@CODE", modules.Code);
                cmd.Parameters.AddWithValue("@LABEL", modules.Label);
                cmd.Parameters.AddWithValue("@MODULES_DESCRIPTION", (object?)(string.IsNullOrWhiteSpace(modules.ModulesDescription) ? null : modules.ModulesDescription.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ICON", (object?)(string.IsNullOrWhiteSpace(modules.Icon) ? null : modules.Icon.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PATH", (object?)(string.IsNullOrWhiteSpace(modules.Path) ? null : modules.Path.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ORDER_NO", modules.OrderNo);
                cmd.Parameters.AddWithValue("@UPDATE_USER", modules.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar módulo.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long moduleId, string status, int updatedBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_MODULES_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@MODULES_ID", moduleId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", updatedBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del módulo.", ex.Message);
            }
        }

        public async Task<bool> ExistsAsync(string label, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var query = new StringBuilder(@"
            SELECT COUNT(*) 
            FROM MODULES 
            WHERE UPPER(LABEL) = UPPER(@LABEL) 
              AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    query.Append(" AND MODULES_ID <> @ID");

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@LABEL", label.Trim());
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0; 
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del módulo.", ex.Message);
            }
        }

    }
}
