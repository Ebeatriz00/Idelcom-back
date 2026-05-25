using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PreSaleProyects
{
    public class PreSaleProyectsResponseDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string ProyectNum { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public string ClientsDescription { get; set; } = string.Empty;
        public string ContactsCrmDescription { get; set; } = string.Empty;
        public string ResponsibleDescription { get; set; } = string.Empty;
        public string SupervisorDescription { get; set; } = string.Empty;
        public string SsomaDescription { get; set; } = string.Empty;
        public string TecLeaderDescription { get; set; } = string.Empty;
        public string OpportunityDescription { get; set; } = string.Empty;
        public string StatePreSaleDescription { get; set; } = string.Empty;
        public string StateColor { get; set; }
        public string QuotationNumberDescription { get; set; } = string.Empty;
        public string OrderNumberDescription { get; set; } = string.Empty;
        public int? NumPercPro {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PreSaleProyectsCount { get; set; }
        public int ObservationsCount { get; set; }
        public string? OpportunityStateDesc { get; set; }
        public string? OpportunityStateColor { get; set; }
        public DateTime? FinishDate { get; set; }
        public string StateGeneralDesc { get; set; }
        public string StateGeneralColor { get; set; }
        public string? OpportunityNumber { get; set; }
        public string? SellerDescription { get; set; }
        public long? ResponsibleId { get; set; }
        public long StatePreSaleId { get; set; }
        public long? ContractObservationsPending { get; set; }
        public int? ProjectCategory { get; set; }
        public int? ContractTotalCount { get; set; }
        public int? CommercialTotalCount { get; set; }
        public int? ObsRevised { get; set; }
        public int? UnreadCommentsCount { get; set; }
        public DateTime? QuoDate { get; set; }
        public int? Category { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CostTotal { get; set; }
        public string? CurrencyDesc { get; set; }
    }
}
