using Core.Entities.Email;
using Core.Interfaces.Notifications;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public class EmailOutboxRepository : IEmailOutboxRepository
    {
        private readonly ISqlConnectionFactory _cn;
        public EmailOutboxRepository(ISqlConnectionFactory cn) => _cn = cn;

        public async Task<List<EmailOutboxItem>> GetPendingBatchAsync(int take, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_EMAIL_OUTBOX_GET_PENDING", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TAKE", take);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            var list = new List<EmailOutboxItem>();

            while (await rd.ReadAsync(ct))
            {
                list.Add(new EmailOutboxItem
                {
                    OutboxId = rd.GetInt64(0),
                    BusinessId = rd.GetInt64(1),
                    EventCode = rd.GetString(2),
                    ToEmail = rd.GetString(3),
                    Subject = rd.GetString(4),
                    PayloadJson = rd.GetString(5),
                    Retries = rd.GetInt32(6),
                    TargetUserId = rd.GetInt64(7),
                    CcEmail = rd.IsDBNull(8) ? null : rd.GetString(8)
                });
            }

            return list;
        }
        public async Task<bool> MarkProcessingAsync(long outboxId, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_EMAIL_OUTBOX_MARK_PROCESSING", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OUTBOX_ID", outboxId);

            var dt = await cmd.ExecuteReaderAsync(ct);
            int affected = 0;

            if (dt.Read())
            {
                affected = Convert.ToInt32(dt["AFFECTED"]);
            }

            return affected == 1;
        }

        public async Task MarkSentAsync(long outboxId, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_EMAIL_OUTBOX_MARK_SENT", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OUTBOX_ID", outboxId);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task MarkFailedAsync(long outboxId, string error, int nextMinutes, CancellationToken ct)
        {
            using var connection = _cn.CreateConnection();
            await connection.OpenAsync(ct);

            using var cmd = new SqlCommand("SP_WS_EMAIL_OUTBOX_MARK_FAILED", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OUTBOX_ID", outboxId);
            cmd.Parameters.AddWithValue("@ERROR", error);
            cmd.Parameters.AddWithValue("@NEXT_MINUTES", nextMinutes);

            await cmd.ExecuteNonQueryAsync(ct);
        }

    }
}
