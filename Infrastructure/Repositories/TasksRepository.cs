using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
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
    public class TasksRepository : ITasksRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public TasksRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string title, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM TASKS WHERE TITLE = @TITLE AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND TASKS_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@TITLE", title);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la tarea.", ex.Message);
            }
        }

        public async Task AddAsync(Tasks entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_TASKS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@TITLE", entity.Title);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", (object?)entity.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PRIORITY_STATE_ID", (entity.PriorityStateId == 0) ? DBNull.Value : entity.PriorityStateId);
                cmd.Parameters.AddWithValue("@TIME", (object?)entity.Time ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PROJECT_ID", (entity.ProjectId ?? 0) == 0 ? DBNull.Value : entity.ProjectId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", (entity.OpporId ?? 0) == 0 ? DBNull.Value : entity.OpporId);
                cmd.Parameters.AddWithValue("@STATE_TASK_ID", entity.StateTaskId);
                cmd.Parameters.AddWithValue("@WORKER_ID", (entity.WorkerId == 0) ? DBNull.Value : entity.WorkerId);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la tarea en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la tarea.", ex.Message);
            }
        }

        public async Task<PagedResult<Tasks>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<Tasks>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_TASKS", cn)
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
                    var tasksId = reader.GetInt64(1);
                    list.Add(new Tasks
                    {
                        BusinessId = reader.GetInt64(0),
                        linkToken = _linkTokenService.Issue("task", tasksId, TimeSpan.FromHours(1)),
                        Title = reader.GetString(2),
                        Description = reader.GetString(3),
                        EndDate = reader.IsDBNull(4) ? null : DateOnly.FromDateTime(reader.GetDateTime(4)),
                        Time = reader.IsDBNull(5) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(5)),
                        Status = reader.GetString(6),
                        OpporDescription = reader.GetString(7),
                        StateTaskDescription = reader.GetString(8),
                        WprkerDescription = reader.GetString(9),
                        PriorityStateDescription = reader.GetString(10),
                        TaskCount = reader.GetInt32(11),
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Tasks>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tareas paginada.", ex.Message);
            }
        }

        public async Task<PagedResult<Tasks>> GetAllProjectsAsync(long businessId, string? search, int page, int pageSize, long? opporId)
        {
            try
            {
                var list = new List<Tasks>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_TASKS_PROJECTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@OPPOR_ID", (object?)opporId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var tasksId = reader.GetInt64(1);
                    long dbOpporId = 0;
                    if (!reader.IsDBNull(12))
                    {
                        dbOpporId = reader.GetInt64(12);
                    }

                    list.Add(new Tasks
                    {
                        BusinessId = reader.GetInt64(0),
                        TasksId = tasksId,
                        linkToken = _linkTokenService.Issue("tasks", tasksId, TimeSpan.FromHours(1)),
                        Title = reader.GetString(2),
                        Description = reader.GetString(3),
                        EndDate = reader.IsDBNull(4) ? null : DateOnly.FromDateTime(reader.GetDateTime(4)),
                        Time = reader.IsDBNull(5) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(5)),
                        Status = reader.GetString(6),
                        ProjectDescription = reader.GetString(7),
                        StateTaskDescription = reader.GetString(8),
                        WprkerDescription = reader.GetString(9),
                        PriorityStateDescription = reader.GetString(10),
                        TaskCount = reader.GetInt32(11),
                        StateTaskId = reader.IsDBNull(13) ? 0 : reader.GetInt64(13),
                        OpporToken = dbOpporId > 0 ? _linkTokenService.Issue("opportunity", dbOpporId, TimeSpan.FromHours(1)) : null,
                        WorkerId = reader.IsDBNull(14) ? 0 : reader.GetInt64(14),
                        PriorityStateId = reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
                        DeliverableId = reader.IsDBNull(16) ? 0 : reader.GetInt64(16)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Tasks>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tareas paginada.", ex.Message);
            }
        }





        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_TASKS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las tareas para el selector.", ex.Message);
            }
        }

        public async Task<Tasks> GetByIdAsync(long tasksId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_TASKS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", tasksId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Tasks
                    {
                        TasksId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Title = reader.GetString(2),
                        Description = reader.GetString(3),
                        EndDate = reader.IsDBNull(4) ? null : DateOnly.FromDateTime(reader.GetDateTime(4)),
                        PriorityStateId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        Time = reader.IsDBNull(6) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(6)),
                        ProjectId = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                        OpporId = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),
                        StateTaskId = reader.GetInt64(9),
                        WorkerId = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la tarea por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Tasks entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_TASKS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", entity.TasksId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@TITLE", entity.Title);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@END_DATE", (object?)entity.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PRIORITY_STATE_ID", entity.PriorityStateId);
                cmd.Parameters.AddWithValue("@TIME", (object?)entity.Time ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PROJECT_ID", (entity.ProjectId ?? 0) == 0 ? DBNull.Value : entity.ProjectId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.OpporId == 0 ? DBNull.Value : (object)entity.OpporId);
                cmd.Parameters.AddWithValue("@STATE_TASK_ID", entity.StateTaskId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la tarea en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long tasksId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_TASKS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", tasksId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la tarea.", ex.Message);
            }
        }

        public async Task<bool> PatchTaskCompletedAsync(string linkToken, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_TASK_COMPLETED", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", linkToken);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la tarea.", ex.Message);
            }
        }
        public async Task<bool> PatchTaskChangeStateAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_TASK_CHANGE_STATE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@TASKS_ID", linkToken);
                cmd.Parameters.AddWithValue("@STATE_TASK_ID", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la tarea.", ex.Message);
            }
        }

        public async Task<bool> PatchTaskPriorityStateAsync(string linkToken, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_TASK_PRIORITY_CHANGE_STATE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@TASKS_ID", linkToken);
                cmd.Parameters.AddWithValue("@PRIORITY_STATE_ID", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la tarea.", ex.Message);
            }
        }
        public async Task<bool> DeleteTaskOpporAsync(string linkToken, string OpporToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_TASK_OPPOR", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", linkToken);
                cmd.Parameters.AddWithValue("@OPPOR_ID", OpporToken);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar la tarea de una oportunidad.", ex.Message);
            }
        }


        public async Task<bool> DeleteTaskProjectAsync(string linkToken, string ProjectToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_TASK_PROJECT", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@TASKS_ID", linkToken);
                cmd.Parameters.AddWithValue("@PROJECT_ID", ProjectToken);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar la tarea de un proyecto.", ex.Message);
            }
        }


    }
}
