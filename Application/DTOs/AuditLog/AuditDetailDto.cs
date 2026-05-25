using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuditLog
{
    public class AuditDetailDto
    {
        public required string FieldName { get; set; }
        public string? FieldAlias { get; set; }
        public string? OldValue { get; set; }
        public required string NewValue { get; set; }
    }
}
