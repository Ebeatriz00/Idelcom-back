using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationWorkOrder : BaseAuditableEntity
    {
        [AuditField("WorkOrderId")]
        public long WorkOrderId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Proyecto")]
        public long OperationsId { get; set; }

        [AuditField("Código de orden de trabajo")]
        public string? WorkOrderCode { get; set; }

        [AuditField("Nombre de orden de trabajo")]
        public string? WorkOrderName { get; set; }

        [AuditField("Estado de la orden")]
        public long OrderStatusId { get; set; }

        [AuditField("Fecha de inicio")]
        public DateTime StartDate { get; set; }

        [AuditField("Fecha de fin")]
        public DateTime? EndDate { get; set; }

        [AuditField("Ubicación")]
        public string? Location { get; set; }

        [AuditField("Requiere logística")]
        public bool NeedLogistics { get; set; }

        [AuditField("Requiere SSOMA")]
        public bool NeedSsoma { get; set; }

        [AuditField("Requiere asistencia")]
        public bool NeedAttendance { get; set; }
        public decimal ProgressPercentage { get; set; }

        [AuditField("Es Administrativo")]
        public bool IsAdministrative { get; set; }

        // Campos auxiliares para la creación automática de Cuadrilla Administrativa
        public long? TechLeaderId { get; set; }
        public string? Description { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}
