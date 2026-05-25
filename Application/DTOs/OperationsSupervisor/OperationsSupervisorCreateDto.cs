using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsSupervisor
{
    public class OperationsSupervisorCreateDto
    {
        public long OperationsId { get; set; }
        public long WorkerId { get; set; }
        public bool IsMain { get; set; }
        public long CreateUser { get; set; }
    }
}
