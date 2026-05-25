using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SubTasksRepository : ISubTasksRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public SubTasksRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task AddAsync(SubTasks entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_SUB_TASKS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@TASKS_ID", entity.TasksId);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)entity.WorkerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TITLE", entity.Title);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", (object?)entity.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TIME", (object?)entity.Time ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_TASK_ID", (object?)entity.StateTaskId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PRIORITY_STATE_ID", (object?)entity.PriorityStateId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la sub-tarea.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(SubTasks entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_SUB_TASKS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@SUB_TASKS_ID", entity.SubTasksId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)entity.WorkerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TITLE", entity.Title);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", (object?)entity.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TIME", (object?)entity.Time ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@STATE_TASK_ID", (object?)entity.StateTaskId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PRIORITY_STATE_ID", (object?)entity.PriorityStateId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la sub-tarea.", ex.Message);
            }
        }

        public async Task<List<SubTasks>> GetAllByTaskAsync(long businessId, long? tasksId)
        {
            try
            {
                var list = new List<SubTasks>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_SUB_TASKS_BY_TASK", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@TASKS_ID", (object?)tasksId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var subTaskId = reader.GetInt64(0);
                    var dbTasksId = reader.GetInt64(14);
                    list.Add(new SubTasks
                    {
                        SubTasksId = subTaskId,
                        LinkToken = _linkTokenService.Issue("subtask", subTaskId, TimeSpan.FromHours(1)),
                        TasksId = dbTasksId,
                        TaskToken = _linkTokenService.Issue("tasks", dbTasksId, TimeSpan.FromHours(1)),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        EndDate = reader.IsDBNull(3) ? null : DateOnly.FromDateTime(reader.GetDateTime(3)),
                        Time = reader.IsDBNull(4) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(4)),
                        WorkerDescription = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        PriorityStateDescription = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        StateTaskDescription = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        AreaDescription = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        WorkerId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        PriorityStateId = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                        StateTaskId = reader.IsDBNull(11) ? null : reader.GetInt64(11),
                        AreaId = reader.IsDBNull(12) ? null : reader.GetInt64(12),
                        Status = reader.GetString(13),
                        BusinessId = reader.GetInt64(15)
                    });
                }
                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar sub-tareas.", ex.Message);
            }
        }

        public async Task<SubTasks> GetByIdAsync(long subTasksId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_SUB_TASKS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SUB_TASKS_ID", subTasksId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SubTasks
                    {
                        SubTasksId = reader.GetInt64(0),
                        TasksId = reader.GetInt64(1),
                        Title = reader.GetString(2),
                        Description = reader.GetString(3),
                        WorkerId = reader.IsDBNull(4) ? null : reader.GetInt64(4),
                        AreaId = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                        StateTaskId = reader.IsDBNull(6) ? null : reader.GetInt64(6),
                        PriorityStateId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                        EndDate = reader.IsDBNull(8) ? null : DateOnly.FromDateTime(reader.GetDateTime(8)),
                        Time = reader.IsDBNull(9) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(9)),
                        Status = reader.GetString(10),
                        BusinessId = reader.GetInt64(11)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener sub-tarea por ID.", ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(long subTasksId, long userId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_SUB_TASKS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SUB_TASKS_ID", subTasksId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", userId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
             

                    var status = reader.GetInt32(1); 
                    return status == 1;
                }

                return false;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar sub-tarea.", ex.Message);
            }
        }
    }
}