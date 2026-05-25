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
    public class ActivityRepository : IActivityRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ActivityRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddActivityOpporAsync(Activity entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_ACTIVITY_OPPOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.OpporToken);
                cmd.Parameters.AddWithValue("@ACTIVITY_TYPE_ID", entity.ActivityType);
                cmd.Parameters.AddWithValue("@WORKER_OWNER_ID", entity.WorkerOwnerId);
                cmd.Parameters.AddWithValue("@WORKER_SENDER_ID", entity.WorkerSenderId);
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", entity.ActivityState);
                cmd.Parameters.AddWithValue("@ACTIVITY_MESSAGE", entity.ActivityMessage);
                cmd.Parameters.AddWithValue("@MESSAGE_ADIC", entity.MessageAddition);
                cmd.Parameters.AddWithValue("@MESSAGE_DATE", entity.MessageDate);
                cmd.Parameters.AddWithValue("@FINISH_DATE", entity.FinishDate);
                cmd.Parameters.AddWithValue("@IND_ENTITY", 1);
                cmd.Parameters.AddWithValue("@IND_PRIORITY_ACT", entity.ActivityPriority);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la actividad de una oportunidad en la base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardarla actividad de una oportunidad.", ex.Message);
            }
        }


        public async Task AddActivityProjectAsync(Activity entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_ACTIVITY_PROJECT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PROJECT_ID", entity.ProjectToken);
                cmd.Parameters.AddWithValue("@ACTIVITY_TYPE_ID", entity.ActivityType);
                cmd.Parameters.AddWithValue("@WORKER_OWNER_ID", entity.WorkerOwnerId);
                cmd.Parameters.AddWithValue("@WORKER_SENDER_ID", entity.WorkerSenderId);
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", entity.ActivityState);
                cmd.Parameters.AddWithValue("@ACTIVITY_MESSAGE", entity.ActivityMessage);
                cmd.Parameters.AddWithValue("@MESSAGE_ADIC", entity.MessageAddition);
                cmd.Parameters.AddWithValue("@MESSAGE_DATE",entity.MessageDate.HasValue ? entity.MessageDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@FINISH_DATE", entity.FinishDate);
                cmd.Parameters.AddWithValue("@IND_ENTITY", 2);
                cmd.Parameters.AddWithValue("@IND_PRIORITY_ACT", entity.ActivityPriority);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la actividad de un proyecto en la base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardarla actividad de un proyecto.", ex.Message);
            }
        }




        public async Task<bool> DeleteActivityOpporAsync(string linkToken, string OpporToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_ACTIVITY_OPPOR", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@ACTIVITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@OPPOR_ID", OpporToken);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar la actividad de una oportunidad.", ex.Message);
            }
        }


        public async Task<bool> DeleteActivityProjectAsync(string linkToken, string ProjectToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_ACTIVITY_PROJECT", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@ACTIVITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@PROJECT_ID", ProjectToken);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar la actividad de un proyecto.", ex.Message);
            }
        }



        public async Task<bool> PatchActivityOpporChangeStateAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_ACTIVITY_STATE_OPPOR", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@ACTIVITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la actividad.", ex.Message);
            }
        }

        public async Task<bool> PatchActivityOpporPriorityStateAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_ACTIVITY_PRIORITY_OPPOR", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@ACTIVITY_ID", linkToken);
                cmd.Parameters.AddWithValue("@IND_PRIORITY_ACT", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la acividad.", ex.Message);
            }
        }

    }
}
