using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quotation
{
    public class SalesQuotationResponseDto
    {
        public string QuotationId { get; set; }
        public string? QuotationNo { get; set; }
        public string? OpporDesc { get; set; }
        public string? ClientsName { get; set; }
        public string? WorkerName { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal? Total { get; set; }
        public string? QuotationStatus { get; set; }
        public string? QuotationColor { get; set; }
        public string? VersionStatus { get; set; }
        public string? VersionColor { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public string? VersionNo { get; set; }
    }
}
