using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsSquad
{
    public class OperationsSquadUpdateDto
    {
        public long SquadId { get; set; }
        public long BusinessId { get; set; }
        public long WorkOrderId { get; set; }
        public string? SquadName { get; set; }
        public long TechLeaderId { get; set; }
        public string? Description { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public string SquadCategory { get; set; } = string.Empty;
        public long UpdateUser { get; set; }
    }
}
