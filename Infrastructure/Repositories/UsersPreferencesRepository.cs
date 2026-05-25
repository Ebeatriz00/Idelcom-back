using Core.Entities;
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
    public class UsersPreferencesRepository : IUsersPreferencesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UsersPreferencesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<UsersPreferences> GetNotifByIdAsync(long usersId, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_NOTIF_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@USERS_ID", usersId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new UsersPreferences
                    {

                        EmailNotif = reader.GetBoolean(0),
                        PushNotif = reader.GetBoolean(1)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario por ID.", ex.Message);
            }
        }

        public async Task<UsersPreferences> GetPreferencesByIdAsync(long usersId, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_PREF_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@USERS_ID", usersId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new UsersPreferences
                    {

                        Language = reader.GetString(0),
                        Timezone = reader.GetString(1)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario por ID.", ex.Message);
            }
        }

        public async Task<UsersPreferences> GetSettingByIdAsync(long usersId, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_USERS_APPEARANCE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@USERS_ID", usersId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new UsersPreferences
                    {

                        Theme = reader.GetString(0),
                        Density = reader.GetString(1)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el usuario por ID.", ex.Message);
            }
        }


        public async Task<bool>  UpdateNotifAsync(UsersPreferences entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_NOTIF", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@EMAIL_NOTIF", SqlDbType.Bit).Value = entity.EmailNotif;
                cmd.Parameters.Add("@PUSH_NOTIF", SqlDbType.Bit).Value = entity.PushNotif;
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }
        public async Task<bool> UpdatePreferencesAsync(UsersPreferences entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_PREF", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@LENGUAGE", SqlDbType.NVarChar).Value = entity.Language;
                cmd.Parameters.Add("@TIMEZONE", SqlDbType.NVarChar).Value = entity.Timezone;
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }
        public async Task<bool> UpdateSettingAsync(UsersPreferences entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_USERS_APPEARANCE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.Int).Value = entity.BusinessId;
                cmd.Parameters.Add("@THEME", SqlDbType.NVarChar).Value = entity.Theme;
                cmd.Parameters.Add("@DENSITY", SqlDbType.NVarChar).Value = entity.Density;
                cmd.Parameters.Add("@USERS_ID", SqlDbType.Int).Value = entity.UsersId;

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el usuario.", ex.Message);
            }
        }
    }
}
