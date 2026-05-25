using Azure;
using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UsersRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> ExistsAsync(string userDocument, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM USERS WHERE USERS_DOCUMENT = @USERSDOC AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND USERS_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@USERSDOC", userDocument);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del usuario.", ex.Message);
            }
        }

        public async Task<bool> ExistsCodeAsync(string usersCode, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM USERS WHERE USERS_CODE = @USERSCODE AND BUSINESS_ID = @BID";


                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@USERSCODE", usersCode);
                cmd.Parameters.AddWithValue("@BID", businessId);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del codigo de usuario.", ex.Message);
            }
        }
        public async Task<bool> GetLastUserCodeAsync(string usersCode, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM USERS WHERE USERS_CODE = @USERSDOCE AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND USERS_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@USERSDOCE", usersCode);
                cmd.Parameters.AddWithValue("@BID", businessId);

                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del usuario.", ex.Message);
            }
        }

        public async Task AddAsync(Users entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_USERS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.Add("@WORKER_ID", SqlDbType.BigInt).Value = entity.WorkerId;
                cmd.Parameters.Add("@USERS_NAME", SqlDbType.VarChar).Value = entity.UsersName;
                cmd.Parameters.Add("@USERS_LAST_NAME", SqlDbType.VarChar).Value = entity.UsersLastName;
                cmd.Parameters.Add("@USERS_CODE", SqlDbType.VarChar).Value = entity.UsersCode;
                cmd.Parameters.Add("@USERS_EMAIL", SqlDbType.VarChar).Value = entity.UsersEmail;
                cmd.Parameters.Add("@DOCUMENT_TYPE_ID", SqlDbType.BigInt).Value = entity.DocumentTypeId;
                cmd.Parameters.Add("@PROFILES_ID", SqlDbType.BigInt).Value = entity.ProfilesId;
                cmd.Parameters.Add("@USERS_DOCUMENT", SqlDbType.VarChar).Value = entity.UsersDocument;
                cmd.Parameters.Add("@USER_PHOTO", SqlDbType.VarChar).Value = entity.UsersPhoto;
                cmd.Parameters.Add("@USERS_PASSWORD", SqlDbType.VarChar).Value = entity.UsersPassword;
                cmd.Parameters.Add("@USERS_SALT", SqlDbType.VarChar).Value = entity.UsersSalt;

                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el usuario en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el usuario.", ex.Message);
            }
        }

        public async Task<PagedResult<Users>> GetAllAsync(int businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<Users>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_LIST", cn)
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
                    list.Add(new Users
                    {
                        BusinessId = reader.GetInt64(0),
                        UsersId = reader.GetInt64(1),
                        User = reader.GetString(2),
                        DocumentType = reader.GetString(3),
                        UsersDocument = reader.GetString(4),
                        DescriptionProfiles = reader.GetString(5),
                        UsersPhoto = reader.GetString(6),
                        Status = reader.GetString(7)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Users>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
                }
                    
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de usuarios.", ex.Message);
            }
        }

        public async Task<Users> GetByIdAsync(long userId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@USERS_ID", userId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Users
                    {

                        BusinessId = reader.GetInt64(0),
                        UsersId = reader.GetInt64(1),
                        UsersName = reader.GetString(2),
                        UsersLastName = reader.GetString(3),
                        DocumentTypeId = reader.GetInt64(4),
                        UsersDocument = reader.GetString(5),
                        ProfilesId = reader.GetInt64(6),
                        UsersPhoto = reader.GetString(7),
                        UsersCode = reader.GetString(8),
                        UsersEmail = reader.GetString(9),
                        WorkerId = reader.IsDBNull(10) ? null : reader.GetInt64(10)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario por ID.", ex.Message);
            }
        }

        public async Task<Users> GetSettingByIdAsync(long userId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_SETTING_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@USERS_ID", userId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Users
                    {

                        UsersId = reader.GetInt64(0),
                        UsersName = reader.GetString(1),
                        UsersLastName = reader.GetString(2),
                        DocumentType = reader.GetString(3),
                        UsersDocument = reader.GetString(4),
                        DescriptionProfiles = reader.GetString(5),
                        UsersPhoto = reader.GetString(6),
                        UsersEmail = reader.GetString(7)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario por ID.", ex.Message);
            }
        }


        public async Task<bool> UpdateAsync(Users entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@WORKER_ID", SqlDbType.BigInt).Value = entity.WorkerId;
                cmd.Parameters.Add("@USERS_NAME", SqlDbType.VarChar).Value = entity.UsersName;
                cmd.Parameters.Add("@USERS_LAST_NAME", SqlDbType.VarChar).Value = entity.UsersLastName;
                cmd.Parameters.Add("@USERS_CODE", SqlDbType.VarChar).Value = entity.UsersCode;
                cmd.Parameters.Add("@USERS_EMAIL", SqlDbType.VarChar).Value = entity.UsersEmail;
                cmd.Parameters.Add("@DOCUMENT_TYPE_ID", SqlDbType.BigInt).Value = entity.DocumentTypeId;
                cmd.Parameters.Add("@PROFILES_ID", SqlDbType.BigInt).Value = entity.ProfilesId;
                cmd.Parameters.Add("@USERS_DOCUMENT", SqlDbType.VarChar).Value = entity.UsersDocument;
                cmd.Parameters.Add("@USERS_PHOTO", SqlDbType.VarChar).Value = entity.UsersPhoto;
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }

        public async Task<bool> UpdateSettingAsync(Users entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_SETTING", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@USERS_NAME", SqlDbType.VarChar).Value = entity.UsersName;
                cmd.Parameters.Add("@USERS_LAST_NAME", SqlDbType.VarChar).Value = entity.UsersLastName;
                cmd.Parameters.Add("@USERS_EMAIL", SqlDbType.VarChar).Value = entity.UsersEmail;
                cmd.Parameters.Add("@USERS_DOCUMENT", SqlDbType.VarChar).Value = entity.UsersDocument;
                cmd.Parameters.Add("@USERS_PHOTO", SqlDbType.VarChar).Value = entity.UsersPhoto;
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }

        public async Task<bool> PasswordChangeAsync(Users entity) {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_PASSWORD", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@USERS_PASSWORD", SqlDbType.VarChar).Value = entity.UsersPassword;
                cmd.Parameters.Add("@USERS_SALT", SqlDbType.VarChar).Value = entity.UsersSalt;
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }
        public async Task<bool> PatchStatusAsync(long userId, string status, int UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@USERS_ID", userId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del usuarios.", ex.Message);
            }
        }
    }
}
