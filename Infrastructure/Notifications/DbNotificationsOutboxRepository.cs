using Core.Entities.Notifications;
using Core.Interfaces.Notifications;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public class DbNotificationsOutboxRepository : INotificationsOutboxRepository
    {
        private readonly ISqlConnectionFactory _cn;
        public DbNotificationsOutboxRepository(ISqlConnectionFactory cn) => _cn = cn;
        public async Task EnqueueAsync(NotificationPersist it, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_NOTIF_PUSH_ENQUEUE", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", it.BusinessId);
            cmd.Parameters.AddWithValue("@USERS_ID", it.UsersId);
            cmd.Parameters.AddWithValue("@TITLE", it.Title);
            cmd.Parameters.AddWithValue("@MESSAGE", it.Message);
            cmd.Parameters.AddWithValue("@MODULE", it.Module);
            cmd.Parameters.AddWithValue("@ENTITY", it.Entity);
            cmd.Parameters.AddWithValue("@ENTITY_ID", long.Parse(it.EntityId));
            cmd.Parameters.AddWithValue("@LINK_URL", (object?)it.LinkUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TYPE", it.Type);
            cmd.Parameters.AddWithValue("@CREATED_BY", it.CreatedBy);

            await cmd.ExecuteNonQueryAsync(ct);
        }
        public async Task<IReadOnlyList<NotificationOutboxRow>> GetPendingAsync(int top, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_OUTBOX_GET_PENDING", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            cmd.Parameters.AddWithValue("@TOP", top);

            var list = new List<NotificationOutboxRow>();

            using var rd = await cmd.ExecuteReaderAsync(ct);
            while (await rd.ReadAsync(ct))
            {
                list.Add(new NotificationOutboxRow
                {
                    OutboxId = rd.GetInt64("OUTBOX_ID"),
                    BusinessId = rd.GetInt64("BUSINESS_ID"),
                    UsersId = rd.GetInt64("USERS_ID"),
                    Title = rd.GetString("TITLE"),
                    Message = rd.GetString("MESSAGE"),
                    Module = rd.GetString("MODULE"),
                    Entity = rd.GetString("ENTITY"),
                    EntityId = rd.GetString("ENTITY_ID"),
                    LinkUrl = rd.IsDBNull("LINK_URL") ? null : rd.GetString("LINK_URL"),
                    Type = rd.GetString("TYPE"),
                    CreatedAt = rd.GetDateTime("CREATED_AT"),
                    CreatedBy = rd.GetInt64("CREATED_BY")
                });
            }

            return list;
        }
        public async Task MarkProcessedAsync(IEnumerable<long> ids, CancellationToken ct)
        {
            var table = new DataTable();
            table.Columns.Add("OUTBOX_ID", typeof(long));

            foreach (var id in ids)
                table.Rows.Add(id);

            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_NOTIF_OUTBOX_MARK_PROCESSED", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            var param = cmd.Parameters.AddWithValue("@ITEMS", table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.NOTIF_OUTBOX_ID_TVP";

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
