using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationsProjectConfig : BaseAuditableEntity
    {
        [AuditField("OperationsProjectConfigId")]
        public long OperationsProjectConfigId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Operaciones")]
        public long OperationsId { get; set; }

        [AuditField("Hora de entrada")]
        public TimeSpan EntryTime { get; set; }

        [AuditField("Hora de salida")]
        public TimeSpan DepartureTime { get; set; }

        [AuditField("Permitir retraso")]
        public bool AllowDelay { get; set; }

        [AuditField("Tolerancia de minutos")]
        public int MinutesTolerance { get; set; }

        [AuditField("Antes de la hora oficial")]
        public TimeSpan BeforeOfficialTime { get; set; }

        [AuditField("Requiere foto")]
        public bool IsRequirePhoto { get; set; }

        [AuditField("Consulta a lo largo del tiempo")]
        public bool IsRequireOvertime { get; set; }

        [AuditField("Requeire aprobacion de horas extras")]
        public bool IsRequireOvertimeApproval { get; set; }

        [AuditField("Turno")]
        public int Shift { get; set; }
    }
}
