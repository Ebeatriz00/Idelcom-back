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

namespace Infrastructure.Persistence.Repositories
{
    public class ProfilesRepository : IProfilesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public ProfilesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> ExistsAsync(string name, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM PROFILES WHERE PROFILES_NAME = @NAME AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    query.Append(" AND PROFILES_ID <> @ID");

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@NAME", name);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new Exceptions.DatabaseException("Error al validar existencia del perfil.", ex.Message);
            }
        }
        public async Task AddAsync(Profiles entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_PROFILES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.Add("@PROFILES_NAME", SqlDbType.VarChar).Value = entity.Name;
                cmd.Parameters.Add("@PROFILES_DESCRIPTION", SqlDbType.VarChar).Value = entity.Description;
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el perfil en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el perfil.", ex.Message);
            }
        }

        public async Task<PagedResult<Profiles>> GetAllAsync(int businessId, string? search,int page, int pageSize)
        {
            try
            {
                var list = new List<Profiles>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_PROFILES_LIST", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                // Primer resultset = items
                while (await reader.ReadAsync())
                {
                    list.Add(new Profiles
                    {
                        BusinessId = reader.GetInt64(0),
                        ProfilesId = reader.GetInt64(1),
                        Name = reader.GetString(2),
                        Description = reader.GetString(3),
                        Status = reader.GetString(4),
                        UsersCount = reader.GetInt32(5)
                    });
                }

                // Segundo resultset = total
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Profiles>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de perfiles paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetProfilesForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_PROFILES_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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

        public async Task<Profiles> GetByIdAsync(long profilesId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_PROFILES_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_ID", profilesId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Profiles
                    {
                        BusinessId = reader.GetInt64(0),
                        ProfilesId = reader.GetInt64(1),
                        Name = reader.GetString(2),
                        Description = reader.GetString(3),
                        Status = reader.GetString(4)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el perfil por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Profiles profiles)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_PROFILES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_ID", profiles.ProfilesId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", profiles.BusinessId);
                cmd.Parameters.AddWithValue("@PROFILES_NAME", profiles.Name);
                cmd.Parameters.AddWithValue("@PROFILES_DESCRIPTION", profiles.Description);
                cmd.Parameters.AddWithValue("@UPDATE_USER", profiles.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el perfil en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long profilesId, string status, int UsersBy, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_UPDATE_PROFILES_ACTIVE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PROFILES_ID", profilesId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del perfil.", ex.Message);
            }
        }
    }
}