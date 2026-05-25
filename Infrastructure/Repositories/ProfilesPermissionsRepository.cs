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
    public class ProfilesPermissionsRepository : IProfilesPermissionsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public ProfilesPermissionsRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(long profilesId, long modulesPermissionsId, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM PROFILES_PERMISSIONS WHERE MODULES_PERMISSIONS_ID = @MPID AND PROFILES_ID = @PID AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    query.Append(" AND PROFILES_PERMISSIONS_ID <> @ID");

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@MPID", modulesPermissionsId);
                cmd.Parameters.AddWithValue("@PID", profilesId);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new Exceptions.DatabaseException("Error al validar existencia del permiso del perfil.", ex.Message);
            }
        }
        public async Task AddAsync(IEnumerable<ProfilesPermissions> entities)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                foreach (var entity in entities)
                {
                    using var cmd = new SqlCommand("SP_WS_REGISTER_PROFILES_PERMISSIONS", connection, tx)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.Add("@PROFILES_ID", SqlDbType.BigInt).Value = entity.ProfilesId;
                    cmd.Parameters.Add("@MODULES_PERMISSIONS_ID", SqlDbType.BigInt).Value = entity.ModulesPermissionsId;
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
        public async Task<PagedResult<ProfilesPermissions>> GetAllAsync( long profilesId, long businessId, int page, int pageSize, string? search = null)
        {
            try
            {
                var list = new List<ProfilesPermissions>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PROFILES_PERMISSIONS_LIST", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_ID", profilesId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                // Primer resultset = items
                while (await reader.ReadAsync())
                {
                    list.Add(new ProfilesPermissions
                    {
                        ProfilesPermissionsId = reader.GetInt64(0),
                        ModulesName = reader.GetString(1),
                        PermissionsName = reader.GetString(2),
                        Status = reader.GetString(3),

                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ProfilesPermissions>
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
        public async Task<ProfilesPermissions?> GetByIdAsync(long profilesPermissionsId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PROFILES_PERMISSIONS_BY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@PROFILES_PERMISSIONS_ID", profilesPermissionsId);

                using var reader = await cmd.ExecuteReaderAsync();

                ProfilesPermissions? result = null;

                while (await reader.ReadAsync())
                {
                    if (result is null)
                    {
                        result = new ProfilesPermissions
                        {
                            BusinessId = reader.GetInt64(0),
                            ProfilesId = reader.GetInt64(1),
                            ModulesPermissionsId = reader.GetInt64(2),
                            ProfilesPermissionsId = reader.GetInt64(3)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener permisos del perfil.", ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(ProfilesPermissions profiles)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PROFILES_PERMISSIONS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_ID", profiles.ProfilesId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", profiles.BusinessId);
                cmd.Parameters.AddWithValue("@PROFILES_ID", profiles.ProfilesId);
                cmd.Parameters.AddWithValue("@MODULES_PERMISSIONS_ID", profiles.ModulesPermissionsId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", profiles.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el permiso del perfil en base de datos.", ex.Message);
            }
        }
        public async Task<bool> PatchStatusAsync(long profilesPermissionsId, string status, long UsersBy, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PROFILES_PERMISSIONS_ACTIVE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_PERMISSIONS_ID", profilesPermissionsId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del permiso del perfil.", ex.Message);
            }
        }



    }
}
