using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationsTeamSsoma : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long OperationsTeamSsomaId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Proceso SSOMA")]
        public long SsomaProcessId { get; set; }

        [AuditField("Tipo de asignamiento")]
        public int? AssignmentId { get; set; }

        [AuditField("Personal SSOMA")]
        public long? WorkerId { get; set; }

        [AuditField("Rol SSOMA")]
        public int SsomaRoleId { get; set; }

        [AuditField("Fecha inicio")]
        public DateOnly StartDate { get; set; }

        [AuditField("Fecha fin")]
        public DateOnly? EndDate { get; set; }

        [AuditField("Líder SSOMA")]
        public bool IsPrimary { get; set; }

        [AuditField("Personal activo")]
        public bool IsActive { get; set; }

        [AuditField("Motivo de cambio")]
        public string? ReasonChange { get; set; }

        [AuditField("Cambio personal SSOMA")]
        public long? ReplacedAssignmentId { get; set; }

        [AuditField("Estado de aprobación cliente")]
        public int? ClientApprovalStatusId { get; set; }

        [AuditField("Fecha de aprobación por el cliente")]
        public DateTime? ClientApprovalDate { get; set; }

        [AuditField("Comentarios")]
        public string? Comments { get; set; }

        [AuditField("Configuración Proyecto")]
        public long? OperationsProjectConfigId { get; set; }
    }
}
