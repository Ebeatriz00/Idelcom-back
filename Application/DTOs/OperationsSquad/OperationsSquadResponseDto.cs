using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsSquad
{
    public class OperationsSquadResponseDto
    {
        public long SquadId { get; set; }
        public long BusinessId { get; set; }
        public long WorkOrderId { get; set; }
        public string WorkOrderName { get; set; }
        public string? SquadName { get; set; }
        public long TechLeaderId { get; set; }
        public string TechLeaderName { get; set; }
        public string? Description { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Status { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public string SquadCategory { get; set; } = string.Empty;
    }
}
