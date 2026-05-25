using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Email
{
    public sealed class EmailOutboxItem
    {
        public long OutboxId { get; set; }
        public  long TargetUserId { get; set; }
        public string EventCode { get; set; } = "";
        public string ToEmail { get; set; } = "";
        public string? CcEmail { get; set; }
        public string Subject { get; set; } = "";
        public string PayloadJson { get; set; } = "";
        public int Retries { get; set; }
        public long BusinessId { get; set; }
    }
}
