using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Audit
{
    public class AuditLogDetail
    {
        public long AuditLogDetailId { get; set; }
        public long AuditLogId { get; set; }
        public long BusinessId { get; set; }
        public string? FieldName { get; set; }
        public string? FieldAlias { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Status { get; set; }
    }
}
