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
    
        public class PermissionsRepository : IPermissionsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public PermissionsRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Permissions entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PERMISSIONS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PERMISSIONS_CODE", entity.PermissionsCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PERMISSIONS_NAME", entity.PermissionsName);
                cmd.Parameters.AddWithValue("@PERMISSIONS_DESCRIPTION", entity.PermissionsDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar permiso en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar permiso.", ex.Message);
            }
        }

        public async Task<PagedResult<Permissions>> GetAllAsync(long businessId, int page, int pageSize)
        {
            try
            {
                var list = new List<Permissions>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_PERMISSIONS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Permissions
                    {
                        BusinessId = reader.GetInt64(0),
                        PermissionsId = reader.GetInt64(1),
                        PermissionsName = reader.GetString(2),
                        PermissionsDescription = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Status = reader.GetString(4),
                        PermissionsCount = reader.GetInt32(5)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Permissions>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de permisos.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetPermissionsForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_PERMISSIONS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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

        public async Task<Permissions> GetByIdAsync(long permissionId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PERMISSION_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PERMISSIONS_ID", permissionId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Permissions
                    {
                        PermissionsId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        PermissionsName = reader.GetString(2),
                        PermissionsDescription = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Status = reader.GetString(4),
                        PermissionsCode = reader.IsDBNull(5) ? null : reader.GetString(5)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener permiso por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Permissions permission)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PERMISSION", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", permission.BusinessId);
                cmd.Parameters.AddWithValue("@PERMISSIONS_ID", permission.PermissionsId);
                cmd.Parameters.AddWithValue("@PERMISSIONS_CODE", permission.PermissionsCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PERMISSIONS_NAME", permission.PermissionsName);
                cmd.Parameters.AddWithValue("@PERMISSIONS_DESCRIPTION", permission.PermissionsDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", permission.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar permiso.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long permissionId, string status, int updatedBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PERMISSION_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PERMISSIONS_ID", permissionId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", updatedBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del permiso.", ex.Message);
            }
        }

        public async Task<bool> ExistsAsync(string permissionName, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var baseQuery = new StringBuilder("SELECT COUNT(*) FROM PERMISSIONS WHERE PERMISSIONS_NAME = @NAME AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    baseQuery.Append(" AND PERMISSIONS_ID <> @ID");

                using var cmd = new SqlCommand(baseQuery.ToString(), connection);
                cmd.Parameters.AddWithValue("@NAME", permissionName);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del permiso.", ex.Message);
            }
        }
    }
}


