using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quotation
{
    public class SalesQuotationVerResponseDto
    {
        public string QuotationVerId { get; set; }
        public string? QuotationNo { get; set; }
        public string? ClientsName { get; set; }
        public string? WorkerResponsible { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal? Total { get; set; }
        public string? VersionStatus { get; set; }
        public string? VersionColor { get; set; }
        public string? VersionNo { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
