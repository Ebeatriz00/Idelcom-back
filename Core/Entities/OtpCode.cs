using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public sealed class OtpCode
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Code { get; init; } = default!; // 6 dígitos
        public int Attempts { get; set; }
        public DateTime ExpiresAtUtc { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public string TenantId { get; init; } = default!;
    }
}
