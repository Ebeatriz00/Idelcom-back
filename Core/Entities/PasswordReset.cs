using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public sealed class PasswordReset
    {
        public Guid Id { get; init; }
        public long UserId { get; init; }
        public string ResetToken { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? ConsumedAtUtc { get; set; }
        public string TenantId { get; init; } = default!;
    }
}
