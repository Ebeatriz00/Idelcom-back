using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class AuthPermisionRepository : IAuthPermisionRepository
    {
        private readonly ISqlConnectionFactory _cnf;
        public AuthPermisionRepository(ISqlConnectionFactory cnf) => _cnf = cnf;

        public async Task<IReadOnlyList<AuthEffectivePerms>> GetEffectiveAsync(long usersId, long businessId, CancellationToken ct = default)
        {
            var list = new List<AuthEffectivePerms>();
            await using var cn = _cnf.CreateConnection();
            await cn.OpenAsync(ct);

            await using var cmd = new SqlCommand("SP_WS_AUTH_EFFECTIVE_PERMS", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = usersId;
            cmd.Parameters.Add("@BusinessId", SqlDbType.BigInt).Value = businessId;

            await using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
            {
                var dto = new AuthEffectivePerms
                {
                    ModulesId = rd.GetInt64(rd.GetOrdinal("MODULES_ID")),
                    ModulesCode = rd.GetString(rd.GetOrdinal("MODULE_CODE")),
                    PermissionsId = rd.GetInt64(rd.GetOrdinal("PERMISSIONS_ID")),
                    PermissionsName = rd.GetString(rd.GetOrdinal("PERMISSION_NAME"))
                };
                list.Add(dto);
            }
            return list;
        }

        public async Task<IReadOnlyList<AuthAllowedModules>> GetAllowedModulesAsync(long usersId, long businessId, CancellationToken ct = default)
        {
            var list = new List<AuthAllowedModules>();
            await using var cn = _cnf.CreateConnection();
            await cn.OpenAsync(ct);

            await using var cmd = new SqlCommand("SP_WS_AUTH_ALLOWED_MODULES", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = usersId;
            cmd.Parameters.Add("@BusinessId", SqlDbType.BigInt).Value = businessId;

            await using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
            {
                var dto = new AuthAllowedModules
                {
                    ModulesId = rd.GetInt64(rd.GetOrdinal("MODULES_ID")),
                    Code = rd.GetString(rd.GetOrdinal("CODE")),
                    Label = rd.IsDBNull(rd.GetOrdinal("LABEL")) ? null : rd.GetString(rd.GetOrdinal("LABEL")),
                    Path = rd.IsDBNull(rd.GetOrdinal("PATH")) ? null : rd.GetString(rd.GetOrdinal("PATH")),
                    IconKey = rd.IsDBNull(rd.GetOrdinal("ICON_KEY")) ? null : rd.GetString(rd.GetOrdinal("ICON_KEY")),
                    ParentModulesId = rd.IsDBNull(rd.GetOrdinal("PARENT_MODULES_ID")) ? (int?)null : rd.GetInt64(rd.GetOrdinal("PARENT_MODULES_ID")),
                    ParentId = rd.IsDBNull(rd.GetOrdinal("PARENT_ID")) ? (int?)null : rd.GetInt64(rd.GetOrdinal("PARENT_ID")),
                    OrderNo = rd.IsDBNull(rd.GetOrdinal("ORDER_NO")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("ORDER_NO"))
                };
                list.Add(dto);
            }
            return list;
        }

        public async Task<AuthCacheKeyInfo> GetCacheKeyInfoAsync(long usersId, long businessId, CancellationToken ct = default)
        {
            await using var cn = _cnf.CreateConnection();
            await cn.OpenAsync(ct);

            long profileId;

            using (var cmd = new SqlCommand(@"
                                                SELECT TOP 1 U.PROFILES_ID
                                                FROM dbo.USERS U WITH (READPAST)
                                                WHERE U.USERS_ID=@UID AND U.BUSINESS_ID=@BID;", cn))
            {
                cmd.Parameters.AddWithValue("@UID", usersId);
                cmd.Parameters.AddWithValue("@BID", businessId);
                profileId = Convert.ToInt64(await cmd.ExecuteScalarAsync(ct) ?? 0L);
            }

            return new AuthCacheKeyInfo { ProfilesId = profileId };
        }
    }
}
