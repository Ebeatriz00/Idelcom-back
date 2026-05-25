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

    namespace Infrastructure.Repositories
    {
        public class ClientsActivityRepository : IClientsActivityRepository
        {
            private readonly ISqlConnectionFactory _connectionFactory;

            public ClientsActivityRepository(ISqlConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }


            public async Task<long> AddActivityAsync(ClientsActivity entity)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    await connection.OpenAsync();

                    using var cmd = new SqlCommand("SP_CRM_CLIENTS_ACTIVITY_CREATE", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    // Parámetros usando las propiedades CORRECTAS de tu Entity
                    cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                    cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                    cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", entity.ActivityStateId);
                    cmd.Parameters.AddWithValue("@ACTIVITY_TYPE_ID", entity.ActivityTypeId);
                    cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);

                    // Manejo de nulos para fecha y descripción
                    cmd.Parameters.AddWithValue("@FINISH_DATE", (object?)entity.FinishDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DESCRIPTION", (object?)entity.Description ?? DBNull.Value);

                    // CORREGIDO: Usamos UsersBy en lugar de CreateUser
                    cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                    var result = await cmd.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt64(result) : 0;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al registrar la actividad del cliente.", ex.Message);
                }
            }

            public async Task<PagedResult<ClientsActivity>> GetActivitiesAllAsync(long businessId, long clientsId, int page, int pageSize)
            {
                try
                {
                    var list = new List<ClientsActivity>();
                    using var connection = _connectionFactory.CreateConnection();
                    await connection.OpenAsync();

                    using var cmd = new SqlCommand("SP_CRM_CLIENTS_ACTIVITY_LIST", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                    cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);
                    cmd.Parameters.AddWithValue("@PAGE", page);
                    cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        list.Add(new ClientsActivity
                        {
                            ClientsActivityId = reader.GetInt64(reader.GetOrdinal("CLIENTS_ACTIVITY_ID")),
                            ClientsId = reader.GetInt64(reader.GetOrdinal("CLIENTS_ID")),
                            Description = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION")) ? string.Empty : reader.GetString(reader.GetOrdinal("DESCRIPTION")),
                            FinishDate = reader.IsDBNull(reader.GetOrdinal("FINISH_DATE")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FINISH_DATE")),
                            WorkerId = reader.GetInt64(reader.GetOrdinal("WORKER_ID")),

                            ActivityTypeId = reader.GetInt32(reader.GetOrdinal("ACTIVITY_TYPE_ID")),
                            Activity = reader.GetString(reader.GetOrdinal("TYPE_NAME")),
                            ActivityIcon = reader.GetString(reader.GetOrdinal("TYPE_ICON")),

                            ActivityStateId = reader.GetInt64(reader.GetOrdinal("ACTIVITY_STATE_ID")),
                            StateDesc = reader.GetString(reader.GetOrdinal("STATE_DESC")),
                            StateColor = reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                            StateIcon = reader.GetString(reader.GetOrdinal("STATE_ICON")),

                            WorkerName = reader.GetString(reader.GetOrdinal("WORKER_NAME"))
                        });
                    }

                    int total = 0;
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        total = reader.GetInt32(0);
                    }

                    return new PagedResult<ClientsActivity>
                    {
                        Items = list,
                        Page = page,
                        PageSize = pageSize,
                        Total = total
                    };
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al listar las actividades del cliente.", ex.Message);
                }
            }

            public async Task<bool> DeleteActivityAsync(long businessId, long clientsActivityId)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    await connection.OpenAsync();

                    using var cmd = new SqlCommand("SP_CRM_CLIENTS_ACTIVITY_DELETE", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                    cmd.Parameters.AddWithValue("@CLIENTS_ACTIVITY_ID", clientsActivityId);

                    using var reader = await cmd.ExecuteReaderAsync();

                    return reader.HasRows && await reader.ReadAsync();
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al eliminar la actividad del cliente.", ex.Message);
                }
            }


            public async Task<bool> UpdateActivityStatusAsync(long businessId, long clientsActivityId, long activityStateId, long usersBy)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    await connection.OpenAsync();

                    using var cmd = new SqlCommand("SP_CRM_CLIENTS_ACTIVITY_UPDATE_STATUS", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                    cmd.Parameters.AddWithValue("@CLIENTS_ACTIVITY_ID", clientsActivityId);
                    cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", activityStateId);
                    cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);

                    var result = await cmd.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) == 1;
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Error al actualizar el estado de la actividad.", ex.Message);
                }
            }


        }
    }
}
