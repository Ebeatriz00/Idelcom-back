using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Ssoma
{
    public class SsomaHomologationPersonnelDocument : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SsomaHomologationPersonnelDocumentId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("personal homologado")]
        public long HomologationPersonnelId { get; set; }

        [AuditField("requerimiento")]
        public int RequirementId { get; set; }

        [AuditField("Nombre de archivo")]
        public string FileName { get; set; } = null!;

        [AuditField("URL del archivo")]
        public string FileUrl { get; set; } = null!;

        [AuditField("Ruta del archivo")]
        public string FilePath { get; set; } = null!;

        [AuditField("Fecha de emisión")]
        public DateTime? IssueDate { get; set; }

        [AuditField("Fecha de expiración")]
        public DateTime? ExpirationDate { get; set; }

        [AuditField("Estado de validación")]
        public int ValidationStatusId { get; set; }

        [AuditField("fecha de revisión")]
        public DateTime? ReviewDate { get; set; } 

        [AuditField("Observación")]
        public string Observation { get; set; } = null!;

        [AuditField("Documento reemplazado")]
        public long? ReplacedDocumentId { get; set; }

        [AuditField("Versión del documento")]
        public int? DocumentVersion { get; set; }

        [AuditField("Motivo del reemplazo")]
        public string? ReplacementReason { get; set; }
    }
}
