using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderCreateDto
    {
        public long OperationsId { get; set; }
        public string? WorkOrderCode { get; set; }
        public string? WorkOrderName { get; set; }
        public long OrderStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public bool NeedLogistics { get; set; }
        public bool NeedSsoma { get; set; }
        public bool NeedAttendance { get; set; }
        public bool IsAdministrative { get; set; }
        public int? ProgressPercentage { get; set; }

        // Campos auxiliares para la creación de Cuadrilla Administrativa en el SP
        public long? TechLeaderId { get; set; }
        public string? Description { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}
