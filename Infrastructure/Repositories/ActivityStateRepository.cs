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
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ActivityStateRepository : IActivityStateRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;
        public ActivityStateRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExistsAsync(string Description, long businessId, string? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM ACTIVITY_STATE WHERE STATE_DESC = @DESC  AND BUSINESS_ID = @BID";

                if (!string.IsNullOrEmpty(excludeId))
                    query += " AND ACTIVITY_STATE_ID <> @ID";

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
                throw new DatabaseException("Error al validar existencia del estado de actividades.", ex.Message);
            }
        }
        public async Task AddAsync(ActivityState entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_STATE_ACTIVITY", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_ICON", entity.StateIcon);
                cmd.Parameters.AddWithValue("@STATE_COLOR", entity.StateColor);
                cmd.Parameters.AddWithValue("@STATE_DESC", entity.StateDesc);
                cmd.Parameters.AddWithValue("@ORDER_STATE", entity.OrderState);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar estado de actividades en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar estado de actividades.", ex.Message);
            }
        }
        public async Task<PagedResult<ActivityState>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<ActivityState>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_STATE_ACTIVITY", cn)
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
                    var likeToken = reader.GetInt64(0);
                    list.Add(new ActivityState
                    {
                        LinkToken = _linkTokenService.Issue("ActivityState", Convert.ToInt64(likeToken), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        StateIcon = reader.GetString(2),
                        StateDesc = reader.GetString(3),
                        OrderState = reader.GetInt32(4),
                        Status = reader.GetString(5)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ActivityState>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de estado de actividades.", ex.Message);
            }
        }
        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_STATE_ACTIVITY_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
        public async Task<ActivityState> GetByIdAsync(long likeToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_STATE_ACTIVITY_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID",likeToken);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    var ActivityState = reader.GetInt64(0);
                    return new ActivityState
                    {
                        LinkToken = _linkTokenService.Issue("ActivityState", Convert.ToInt64(ActivityState), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(1),
                        StateIcon = reader.GetString(2),
                        StateDesc = reader.GetString(3),
                        OrderState = reader.GetInt32(4)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener estado de actividades por ID.", ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(ActivityState entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_STATE_ACTIVITY", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", entity.LinkToken);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_ICON", entity.StateIcon);
                cmd.Parameters.AddWithValue("@STATE_COLOR", entity.StateColor);
                cmd.Parameters.AddWithValue("@STATE_DESC", entity.StateDesc);
                cmd.Parameters.AddWithValue("@ORDER_STATE", entity.OrderState);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar estado de actividades.", ex.Message);
            }
        }
        public async Task<bool> PatchStatusAsync(string likeToken, string status, long UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_ACTIVITY_STATE_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@ACTIVITY_STATE_ID", likeToken);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado de estado de actividades.", ex.Message);
            }
        }

        public async Task<List<ActivityState>> GetSelectAsync(long businessId)
        {
            var list = new List<ActivityState>();

            try
            {

                await using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_STATE_ACTIVITY_SELECT_NORMAL", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var idValue = reader["ACTIVITY_STATE_ID"];
                    var idStr = Convert.ToString(idValue);

                    list.Add(new ActivityState
                    {
                        LinkToken = _linkTokenService.Issue("ActivityState", Convert.ToInt64(idStr), TimeSpan.FromHours(1)),
                        StateColor = reader.GetString(1),
                        OrderState = reader.GetInt32(2),
                        StateDesc = reader.GetString(3)

                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la lista de estado de actividades activos.", ex.Message);
            }
        }
    }
}
