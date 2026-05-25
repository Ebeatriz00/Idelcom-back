using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Application.DTOs.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressSyncDto
    {
        public long ActivityId { get; set; }
        public decimal ReportedQuantity { get; set; }
        public DateTime ReportedDate { get; set; }
        public long? WorkerId { get; set; }
        public string? Observations { get; set; }

        /// <summary>
        /// Identificador único generado por la aplicación móvil para garantizar idempotencia.
        /// </summary>
        public string? AppRecordId { get; set; }

        /// <summary>
        /// Colección de evidencias fotográficas (máximo 5 sugerido).
        /// </summary>
        public List<IFormFile>? Photos { get; set; }
    }
}
