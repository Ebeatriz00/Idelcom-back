using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly IPasswordService _passwordService;

        public AuthRepository(ISqlConnectionFactory connectionFactory, IPasswordService passwordService)
        {
            _connectionFactory = connectionFactory;
            _passwordService = passwordService;
        }

        public async Task<Auth> AuthenticateAsync(string usersKeyNormalized, CancellationToken ct = default)
        {
            var resp = new Auth { Status = "0", Message = "Usuario no encontrado." };

            try
            {
                await using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync(ct);

                // 1) LOOKUP PÚBLICO (no expone hash/salt) + USER_PHOTO
                await using (var cmd = new SqlCommand("SP_WS_AUTH_LOOKUP_PUBLIC", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                })
                {
                    var p = cmd.Parameters.Add("@USER_KEY", SqlDbType.VarChar, 320);
                    p.Value = usersKeyNormalized ?? string.Empty;

                    await using var rd = await cmd.ExecuteReaderAsync(ct);

                    // RS#1: datos públicos
                    if (await rd.ReadAsync(ct))
                    {
                        int O(string name) => rd.GetOrdinal(name);
                        long GetInt64(string name) =>
                            rd.IsDBNull(O(name)) ? 0L : Convert.ToInt64(rd.GetValue(O(name)), CultureInfo.InvariantCulture);
                        string? GetStr(string name) => rd.IsDBNull(O(name)) ? null : rd.GetString(O(name));

                        resp.UsersId = GetInt64("USERS_ID");
                        resp.UsersName = GetStr("USERS_NAME");
                        resp.UsersLastName = GetStr("USERS_LAST_NAME");
                        resp.FullName = GetStr("FULL_NAME");
                        resp.UsersEmail = GetStr("USERS_EMAIL");
                        resp.UserPhoto = GetStr("USER_PHOTO");           
                        resp.ProfilesId = GetInt64("PROFILES_ID");
                        resp.ProfilesName = GetStr("PROFILE_NAME");
                        resp.BusinessId = GetInt64("BUSINESS_ID");
                        resp.BusinessName = GetStr("BUSINESS_NAME");
                        resp.BusinessRuc = GetStr("BUSINESS_RUC");
                        resp.CodeLicense = GetStr("CODE_LICENSE");
                        resp.WorkerId = rd.IsDBNull(O("WORKER_ID")) ? null : GetInt64("WORKER_ID");
                        resp.AreasId = GetInt64("AREA_ID");
                        resp.UsersVisibiliyId = GetInt64("INTERNAL_VISIBILITY_ID");
                    }

                    // RS#2: message/status
                    if (await rd.NextResultAsync(ct) && await rd.ReadAsync(ct))
                    {
                        int O2(string name) => rd.GetOrdinal(name);
                        resp.Message = rd.IsDBNull(O2("message")) ? resp.Message : rd.GetString(O2("message"));
                        resp.Status = rd.IsDBNull(O2("status")) ? resp.Status : Convert.ToInt32(rd.GetValue(O2("status")), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    }
                }

                // Si no encontró usuario, corta
                if (resp.Status != "1" || resp.UsersId <= 0)
                    return resp;

                // 2) SECRETOS (hash/salt) para validación local del password
                await using (var cmdSec = new SqlCommand("SP_AUTH_SECRET_BY_USER", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                })
                {
                    cmdSec.Parameters.Add("@USER_ID", SqlDbType.BigInt).Value = resp.UsersId;

                    await using var rd2 = await cmdSec.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
                    if (await rd2.ReadAsync(ct))
                    {
                        int O(string n) => rd2.GetOrdinal(n);
                        resp.UsersPassword = rd2.IsDBNull(O("PASSWORD_HASH")) ? string.Empty : rd2.GetString(O("PASSWORD_HASH"));
                        resp.UsersSalt = rd2.IsDBNull(O("SALT")) ? string.Empty : rd2.GetString(O("SALT"));
                    }
                }

                // 3) Estado final
                if (string.IsNullOrEmpty(resp.UsersPassword) || string.IsNullOrEmpty(resp.UsersSalt))
                {
                    // por seguridad, si no hay secretos válidos
                    resp.Status = "0";
                    resp.Message = "Usuario sin credenciales válidas.";
                }
                else
                {
                    // listo para validar contraseña y emita JWT
                    if (string.IsNullOrEmpty(resp.Message)) resp.Message = "Usuario encontrado.";
                    resp.Status = "1";
                }

                return resp;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }
    }
}
