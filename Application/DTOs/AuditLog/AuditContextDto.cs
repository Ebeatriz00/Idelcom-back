using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuditLog
{
    public class AuditContextDto
    {
        public long BusinessId { get; set; }
        public required string TableName { get; set; }
        public long RecordId { get; set; }
        public required string ActionType { get; set; }
        public string? ModuleName { get; set; }
        public string? Comment { get; set; }
        public long CreateUser { get; set; }
        public string? IpAddress { get; set; }
    }
}
