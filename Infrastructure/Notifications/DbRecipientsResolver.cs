using Core.Entities.Notifications;
using Core.Entities.paginations;
using Core.Interfaces.Notifications;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace Infrastructure.Notifications
{
    public sealed class DbRecipientsResolver : IRecipientsResolver
    {
        private readonly ISqlConnectionFactory _cn;

        public DbRecipientsResolver(ISqlConnectionFactory cn) => _cn = cn;

        public async Task<IReadOnlyList<long>> ResolveAsync(NotificationEvent ev, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIFICATION_RESOLVER", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EVENT_CODE", ev.Type);
            cmd.Parameters.AddWithValue("@JSON", JsonSerializer.Serialize(ev, ev.GetType()));


            var list = new List<long>();
            using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
                list.Add(rd.GetInt64(0));

            return list;
        }

        public async Task<PagedResult<NotificationPersist>> ListForUserAsync(long businessId, long userId, string? search, int page, int pageSize, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_LIST_FOR_USER", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@USERS_ID", userId);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
            cmd.Parameters.AddWithValue("@SEARCH",
                string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search);

            var items = new List<NotificationPersist>();
            int totalRows = 0;

            using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
            {
                if (totalRows == 0)
                {
                    totalRows = rd.GetInt32(rd.GetOrdinal("TOTAL_ROWS"));
                }

                items.Add(new NotificationPersist
                {
                    NotificationId = rd.GetInt64(rd.GetOrdinal("NOTIFICATION_ID")),
                    BusinessId = rd.GetInt64(rd.GetOrdinal("BUSINESS_ID")),
                    UsersId = rd.GetInt64(rd.GetOrdinal("USERS_ID")),
                    Title = rd.GetString(rd.GetOrdinal("TITLE")),
                    Message = rd.GetString(rd.GetOrdinal("MESSAGE")),
                    Module = rd.GetString(rd.GetOrdinal("MODULE")),
                    Entity = rd.GetString(rd.GetOrdinal("ENTITY")),
                    EntityId = rd.GetString(rd.GetOrdinal("ENTITY_ID")),
                    LinkUrl = rd.IsDBNull(rd.GetOrdinal("LINK_URL"))
                                       ? null
                                       : rd.GetString(rd.GetOrdinal("LINK_URL")),
                    Type = rd.GetString(rd.GetOrdinal("TYPE")),
                    ReadAt = rd.IsDBNull(rd.GetOrdinal("READ_AT"))
                                       ? null
                                       : rd.GetDateTime(rd.GetOrdinal("READ_AT")),
                    CreatedBy = rd.GetInt64(rd.GetOrdinal("CREATED_BY")),
                    CreatedAt = rd.GetDateTime(rd.GetOrdinal("CREATED_AT")),
                    CreatedByName = rd.GetString(rd.GetOrdinal("CREATED_BY_NAME")),
                    FinishDate = rd.IsDBNull(rd.GetOrdinal("FINISH_DATE"))
                                       ? null
                                       : rd.GetDateTime(rd.GetOrdinal("FINISH_DATE")),
                });
            }

            return new PagedResult<NotificationPersist>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                 Total = totalRows
            };
        }

        public async Task AddManyAsync(IEnumerable<NotificationPersist> items, CancellationToken ct)
        {
            var table = new DataTable();
            table.Columns.Add("BUSINESS_ID", typeof(long));
            table.Columns.Add("USERS_ID", typeof(long));
            table.Columns.Add("TITLE", typeof(string));
            table.Columns.Add("MESSAGE", typeof(string));
            table.Columns.Add("MODULE", typeof(string));
            table.Columns.Add("ENTITY", typeof(string));
            table.Columns.Add("ENTITY_ID", typeof(long));
            table.Columns.Add("LINK_URL", typeof(string));
            table.Columns.Add("TYPE", typeof(string));
            table.Columns.Add("CREATED_BY", typeof(long));

            foreach (var it in items)
            {
                table.Rows.Add(
                    it.BusinessId,
                    it.UsersId,
                    it.Title,
                    it.Message,
                    it.Module,
                    it.Entity,
                    it.EntityId,
                    it.LinkUrl,
                    it.Type,
                    it.CreatedBy
                );
            }

            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_BULK_INSERT", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var p = cmd.Parameters.AddWithValue("@ITEMS", table);
            p.SqlDbType = SqlDbType.Structured;
            p.TypeName = "dbo.NOTIF_TVP";

            await cmd.ExecuteNonQueryAsync(ct);
        }
        public async Task MarkAllRead(long businessId, long usersId, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_MARK_ALL_READ", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@USERS_ID", usersId);

            await cmd.ExecuteNonQueryAsync(ct);
        }
        public async Task MarkRead(long businessId, long usersId, long notificationId, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_MARK_READ", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@USERS_ID", usersId);
            cmd.Parameters.AddWithValue("@NOTIFICATIONS_ID", notificationId);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task AlertResolve(long businessId, long usersId, long notificationId, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_OPPOR_ALERT_RESOLVE", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@ALERT_ID", notificationId);
            cmd.Parameters.AddWithValue("@USER_ID", usersId);
            cmd.Parameters.AddWithValue("@COMMENT", "-");

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task AlertSnooze(long businessId, long usersId, long notificationId, DateTime snoozeUntil, string comment, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_OPPOR_ALERT_SNOOZE", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@ALERT_ID", notificationId);
            cmd.Parameters.AddWithValue("@SNOOZED_UNTIL", snoozeUntil);
            cmd.Parameters.AddWithValue("@COMMENT", comment);
            cmd.Parameters.AddWithValue("@USER_ID", usersId);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }

}