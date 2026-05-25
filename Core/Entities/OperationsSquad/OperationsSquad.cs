using Core.Attributes;

namespace Core.Entities.OperationsSquad
{
    public class OperationSquad
    {
        public long SquadId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Id Orden de Trabajo")]
        public long WorkOrderId { get; set; }

        [AuditField("Orden de Trabajo")]
        public string WorkOrderName { get; set; }

        [AuditField("Nombre de Cuadrilla")]
        public string? SquadName { get; set; }

        [AuditField("Id Técnico Líder")]
        public long TechLeaderId { get; set; }

        [AuditField("Técnico Líder")]
        public string TechLeaderName { get; set; }

        [AuditField("Descripción")]
        public string? Description { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Status { get; set; }
        [AuditField("Configuración de Proyecto")]
        public long? OperationsProjectConfigId { get; set; }
        
        [AuditField("Categoría de Cuadrilla")]
        public string SquadCategory { get; set; } = string.Empty;
    }
}
