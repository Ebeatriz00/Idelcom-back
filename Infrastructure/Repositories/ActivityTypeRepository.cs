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
    public class ActivityTypeRepository : IActivityTypeRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;
        public ActivityTypeRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string Description, long businessId, string? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM ACTIVITY_TYPE WHERE ACTIVITY = @DESC  AND BUSINESS_ID = @BID";

                if (!string.IsNullOrEmpty(excludeId))
                    query += " AND ACTIVITY_TYPE_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DESC", Description);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (!string.IsNullOrEmpty(excludeId))
                    cmd.Parameters.AddWithValue("@ID", excludeId);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del tipo de actividades.", ex.Message);
            }
        }
        public async Task AddAsync(ActivityType entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_ACTIVITY_TYPE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@ACTIVITY_ICON", entity.ActivityIcon);
                cmd.Parameters.AddWithValue("@ACTIVITY", entity.ActivityDesc);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar  tipo de actividades en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar tipo de actividades.", ex.Message);
            }
        }
        public async Task<PagedResult<ActivityType>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<ActivityType>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_ACTIVITY_TYPE", cn)
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
                    var ActivityTypeId = reader.GetInt64(0);
                    list.Add(new ActivityType
                    {
                        LinkToken = _linkTokenService.Issue("ActivityType", Convert.ToInt64(ActivityTypeId), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        ActivityIcon = reader.GetString(2),
                        ActivityDesc = reader.GetString(3),
                        Status = reader.GetString(4)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ActivityType>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipo de actividades.", ex.Message);
            }
        }
        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_ACTIVITY_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt32(0),
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
        public async Task<ActivityType> GetByIdAsync(string likeToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_ACTIVITY_TYPE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", likeToken);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    var ActivityType = reader.GetInt64(0);
                    return new ActivityType
                    {
                        LinkToken = _linkTokenService.Issue("ActivityType", Convert.ToInt64(ActivityType), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        ActivityIcon = reader.GetString(2),
                        ActivityDesc = reader.GetString(3)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener tipo de actividades por ID.", ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(ActivityType entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_ACTIVITY_TYPE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@STATE_TASK_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@ACTIVITY_ICON", entity.ActivityIcon);
                cmd.Parameters.AddWithValue("@ACTIVITY", entity.ActivityDesc);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar tipo de actividades.", ex.Message);
            }
        }
        public async Task<bool> PatchStatusAsync(string likeToken, string status, long UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_ACTIVITY_TYPE_ACTIVITE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@ACTIVITY_TYPE_ID", likeToken);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado de tipo de actividades.", ex.Message);
            }
        }

        public async Task<List<ActivityType>> GetSelectAsync(long businessId)
        {
            var list = new List<ActivityType>();

            try
            {

                await using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_STATE_TASK_SELECT_NORMAL", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var idValue = reader["STATE_TASK_ID"];
                    var idStr = Convert.ToString(idValue);

                    list.Add(new ActivityType
                    {
                        LinkToken = _linkTokenService.Issue("ActivityType", Convert.ToInt64(idStr), TimeSpan.FromHours(1)),
                        ActivityIcon = reader.GetString(1),
                        ActivityDesc = reader.GetString(2)

                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la lista de tipo de actividades activos.", ex.Message);
            }
        }
    }
}
