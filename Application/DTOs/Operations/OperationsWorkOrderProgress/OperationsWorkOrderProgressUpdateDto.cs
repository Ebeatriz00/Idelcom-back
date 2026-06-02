using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressUpdateDto
    {
        public long ProgressId { get; set; }
        public decimal ReportedQuantity { get; set; }
        public DateTime ReportedDate { get; set; }
        public string? Observations { get; set; }

        /// <summary>
        /// Identificador único generado por la aplicación móvil para garantizar idempotencia.
        /// </summary>
        public string? AppRecordId { get; set; }

        /// <summary>
        /// Colección de evidencias fotográficas actualizadas (máximo 5 sugerido).
        /// </summary>
        public List<IFormFile>? Photos { get; set; }
    }
}
