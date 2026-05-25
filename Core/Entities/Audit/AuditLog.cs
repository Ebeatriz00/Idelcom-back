using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Audit
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }
        public long BusinessId { get; set; }
        public required string TableName { get; set; }
        public long RecordId { get; set; }
        public string? ActionType { get; set; }
        public string? ModuleName { get; set; }
        public string? IpAddress { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUser { get; set; }
        public string? Status { get; set; }
    }
}
