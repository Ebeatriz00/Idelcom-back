using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quotation
{
    public class SalesQuotationVerCreateDto
    {
        public long BusinessId { get; set; }

        public long? OpporId { get; set; }

        
        public long? ClientsId { get; set; }
        public string? ClientsName { get; set; }

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
