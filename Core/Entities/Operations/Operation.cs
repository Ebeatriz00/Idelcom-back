using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class Operation : BaseAuditableEntity
    {
        [AuditField("OperationsId")]
        public long OperationsId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Oportunidad")]
        public long OpporId { get; set; }

        [AuditField("Supervisor de calidad")]
        public long? QualitySupervisorId { get; set; }

        [AuditField("Jefe de proyecto")]
        public long? ProjectManagerId { get; set; }

        [AuditField("Requiere SSOMA")]
        public bool? RequeredSsoma { get; set; }

        [AuditField("Fecha planificada de inicio")]
        public DateTime? PlannedStartDate { get; set; }

        [AuditField("Fecha real de inicio")]
        public DateTime? ActualStartDate { get; set; }

        [AuditField("Fecha planificada de fin")]
        public DateTime? PlannedEndDate { get; set; }

        [AuditField("Fecha real de fin")]
        public DateTime? ActualEndDate { get; set; }

        [AuditField("Estado de operación")]
        public int? OperationsStatusId { get; set; }
        public decimal? ProgressPercentage { get; set; }

        [AuditField("Acta de Cierre UID")]
        public Guid? ClosurePdfFileUid { get; set; }
    }
}
