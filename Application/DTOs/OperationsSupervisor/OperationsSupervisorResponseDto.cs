using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsSupervisor
{
    public class OperationsSupervisorResponseDto
    {
        public long SupervisorId { get; set; }
        public long OperationsId { get; set; }
        public long WorkerId { get; set; }
        public bool IsMain { get; set; }
        public string SupervisorName { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Status { get; set; }
    }
}
