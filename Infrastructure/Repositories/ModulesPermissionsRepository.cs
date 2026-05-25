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
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ModulesPermissionsRepository : IModulesPermissionsRepository
    {
            private readonly ISqlConnectionFactory _connectionFactory;

            public ModulesPermissionsRepository(ISqlConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public async Task AddAsync(ModulesPermissions entity)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var cmd = new SqlCommand("SP_WS_REGISTER_MODULES_PERMISSIONS", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@MODULES_ID", entity.ModulesId);
                    cmd.Parameters.AddWithValue("@PERMISSIONS_ID", entity.PermissionsId);
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al registrar permiso de módulo.", ex.Message);
                }
            }

            public async Task<PagedResult<ModulesPermissions>> GetAllAsync(long businessId,int page, int pageSize, string? search = null)
            {
                try
                {
                    using var cn = _connectionFactory.CreateConnection();
                    await cn.OpenAsync();

                    var items = new List<ModulesPermissions>();
                    int total = 0;

                // 1) Resumen por módulo (paginado)
                using (var cmd = new SqlCommand("SP_WS_LIST_MODULES_SUMMARY", cn)
                { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 })
                {
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                    cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PAGE", page);
                    cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                    using var rd = await cmd.ExecuteReaderAsync();

                    // resultset 1: filas
                    if (rd.HasRows)
                    {
                        int oModulesPermissionsId = rd.GetOrdinal("MODULES_PERMISSIONS_ID");
                        int oModulesId = rd.GetOrdinal("MODULES_ID");
                        int oModulesName = rd.GetOrdinal("MODULES_NAME");
                        int oModulesDescription = rd.GetOrdinal("MODULES_DESCRIPTION");
                        int oStatus = rd.GetOrdinal("STATUS");
                        int oTotalPerms = rd.GetOrdinal("TOTAL_PERMISSIONS");
                        int oActivePerms = rd.GetOrdinal("ACTIVE_PERMISSIONS");
                        int oUsedInProfiles = rd.GetOrdinal("USED_IN_PROFILES");

                        while (await rd.ReadAsync())
                        {
                            items.Add(new ModulesPermissions
                            {
                                ModulesPermissionsId = rd.IsDBNull(oModulesPermissionsId) ? 0 : rd.GetInt64(oModulesPermissionsId),
                                ModulesId = rd.IsDBNull(oModulesId) ? 0 : rd.GetInt64(oModulesId),
                                ModulesName = rd.IsDBNull(oModulesName) ? "" : rd.GetString(oModulesName),
                                ModulesDescription = rd.IsDBNull(oModulesDescription) ? "" : rd.GetString(oModulesDescription),
                                Status = rd.IsDBNull(oStatus) ? "SIN ESTADO" : rd.GetString(oStatus),
                                TotalPermissions = rd.IsDBNull(oTotalPerms) ? 0 : rd.GetInt32(oTotalPerms),
                                ActivePermissions = rd.IsDBNull(oActivePerms) ? 0 : rd.GetInt32(oActivePerms),
                                UsedInProfiles = rd.IsDBNull(oUsedInProfiles) ? 0 : rd.GetInt32(oUsedInProfiles),
                            });
                        }
                    }

                    // resultset 2: total
                    if (await rd.NextResultAsync() && await rd.ReadAsync())
                    {
                        int oTotal = rd.GetOrdinal("TOTAL_COUNT");
                        total = rd.IsDBNull(oTotal) ? 0 : rd.GetInt32(oTotal);
                    }
                }

                // 2) Para cada módulo de la página, cargar sus permisos
                foreach (var m in items)
                {
                    using var cmdPerm = new SqlCommand("SP_WS_LIST_PERMISSIONS_BY_MODULES", cn)
                    { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };

                    cmdPerm.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;
                    cmdPerm.Parameters.Add("@MODULES_ID", SqlDbType.BigInt).Value = m.ModulesId;

                    using var r2 = await cmdPerm.ExecuteReaderAsync();
                    if (r2.HasRows)
                    {
                        int oId = r2.GetOrdinal("permissionsId");
                        int oName = r2.GetOrdinal("permisionsName");
                        int oMpId = r2.GetOrdinal("mpid");
                        while (await r2.ReadAsync())
                        {
                            var idVal = r2.IsDBNull(oId) ? 0L : Convert.ToInt64(r2.GetValue(oId));
                            var nameVal = r2.IsDBNull(oName) ? "" : r2.GetString(oName);
                            var id = r2.IsDBNull(oMpId) ? 0L : Convert.ToInt64(r2.GetValue(oMpId));
                            m.ListModulesPermissions.Add(new ListPermissions
                            {
                                PermissionsId = idVal,
                                PermissionsName = nameVal,
                                ModulesPermissionsId = id
                            });
                        }
                    }
                }

                return new PagedResult<ModulesPermissions>
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al obtener lista de permisos de módulos.", ex.Message);
                }
            }

            public async Task<ModulesPermissions> GetByIdAsync(long modulesPermissionsId)
            {
                try
                {
                    using var cn = _connectionFactory.CreateConnection();
                    await cn.OpenAsync();

                    using var cmd = new SqlCommand("SP_WS_MODULES_PERMISSIONS_BY_ID", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };
                    cmd.Parameters.AddWithValue("@MODULES_PERMISSIONS_ID", modulesPermissionsId);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        return new ModulesPermissions
                        {
                            ModulesPermissionsId = reader.GetInt64(0),
                            BusinessId = reader.GetInt64(1),
                            ModulesId = reader.GetInt64(2),
                            PermissionsId = reader.GetInt64(3),
                            Status = reader.GetString(4)
                        };
                    }

                    return null;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al obtener permiso de módulo por ID.", ex.Message);
                }
            }

            public async Task<bool> UpdateAsync(ModulesPermissions entity)
            {
                try
                {
                    using var cn = _connectionFactory.CreateConnection();
                    await cn.OpenAsync();

                    using var cmd = new SqlCommand("SP_WS_UPDATE_MODULES_PERMISSIONS", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@MODULES_PERMISSIONS_ID", entity.ModulesPermissionsId);
                    cmd.Parameters.AddWithValue("@MODULES_ID", entity.ModulesId);
                    cmd.Parameters.AddWithValue("@PERMISSIONS_ID", entity.PermissionsId);
                    cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                    using var reader = await cmd.ExecuteReaderAsync();
                    return reader.HasRows && await reader.ReadAsync();
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al actualizar permiso de módulo.", ex.Message);
                }
            }

            public async Task<bool> PatchStatusAsync(long modulesPermissionsId, string status, long UsersBy, long businessId)
            {
                try
                {
                    using var cn = _connectionFactory.CreateConnection();
                    await cn.OpenAsync();

                    using var cmd = new SqlCommand("SP_WS_UPDATE_MODULES_PERMISSIONS_STATUS", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                    cmd.Parameters.AddWithValue("@MODULES_PERMISSIONS_ID", modulesPermissionsId);
                    cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                    cmd.Parameters.AddWithValue("@STATUS", status);

                    using var reader = await cmd.ExecuteReaderAsync();
                    return reader.HasRows && await reader.ReadAsync();
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al cambiar el estado del permiso de módulo.", ex.Message);
                }
            }

            public async Task<bool> ExistsAsync(long modulessId, long permissionsId, long businessId, long? excludeId = null)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();

                    var baseQuery = new StringBuilder("SELECT COUNT(*) FROM MODULES_PERMISSIONS WHERE MODULES_ID = @MID AND PERMISSIONS_ID = @PID AND BUSINESS_ID = @BID");

                    if (excludeId.HasValue)
                        baseQuery.Append(" AND MODULES_PERMISSIONS_ID <> @ID");

                    using var cmd = new SqlCommand(baseQuery.ToString(), connection);
                    cmd.Parameters.AddWithValue("@MID", modulessId);
                    cmd.Parameters.AddWithValue("@PID", permissionsId);
                    cmd.Parameters.AddWithValue("@BID", businessId);
                    if (excludeId.HasValue)
                        cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                    await connection.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al validar existencia del permiso de módulo.", ex.Message);
                }
            }

           
    }

}
