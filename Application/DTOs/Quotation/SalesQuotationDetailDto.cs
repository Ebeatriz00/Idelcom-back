using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quotation
{
    public class SalesQuotationDetailDto
    {
        public string QuotationVerId { get; set; }
        public long BusinessId { get; set; }
        public string? VersionNo { get; set; }


        public long? OpporId { get; set; }
        public string? OpporNumber { get; set; }
        public string? OpporName { get; set; }
        public long? ClientsId { get; set; }
        public string? ClientsName { get; set; }
        public long? QuotationStatusId { get; set; }

        public long? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public decimal? ExchangeRate { get; set; }

        public long? PaymentConditionId { get; set; }
        public string? PaymentConditionName { get; set; }
        public int? OfferValidity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public int? CurrentVersionNo { get; set; }
        public long? VersionStatusId { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Total { get; set; }

        public decimal? SalesTotal { get; set; }
        public decimal? CostTotal { get; set; }
        public decimal? UtilityTotal { get; set; }
        public decimal? MarginPercent { get; set; }

        //detalles
        public List<SalesQuotationLinDto>? Lines { get; set; }

        //--Margenes
        public List<SalesQuotationMarginDto>? Margins { get; set; }
        //Comprobacion de servicios
        public List<SalesQuotationServCheckDto>? ServChecks { get; set; }
        // Pagos planificados
        public List<SalesQuotationLinePlanDto>? LinePlans { get; set; }
        // Egresos planificados
        public List<SalesQuotationEgressDto>? Egresses { get; set; }
        public long UsersBy { get; set; }

    }
    
}
