using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quotation
{
    public class SalesQuotationCreateDto
    {
        public long BusinessId { get; set; }

        public long? OpporId { get; set; }
        public string? OpporName { get; set; }
        public string? OpporNumber { get; set; }

        public long? ClientsId { get; set; }
        public string? ClientsName { get; set; }


        public long? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public decimal? ExchangeRate { get; set; }

        public long? PaymentConditionId { get; set; }
        public string? PaymentConditionName { get; set; }

        public int? OfferValidity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Total { get; set; }

        public decimal? VvcTotal { get; set; }
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
    public class SalesQuotationLinDto
    {
        public long QuotationVerId { get; set; }
        public long? QuotationVerLinId { get; set; }
        public int? LineNo { get; set; }
        public string? DisplayNo { get; set; }

        public string? LineType { get; set; }
        public int? LevelNo { get; set; }
        public bool? IsRollUp { get; set; }
        public long? ParentQuotationVerLinId { get; set; }
        

        public long? ItemId { get; set; }
        public long? ProductsId { get; set; }
        public string? Description { get; set; }

        public long? ProductsTypeId { get; set; }
        public string? ProductsTypeName { get; set; }

        public long? UomId { get; set; }
        public string? UomName { get; set; }

        public long? BrandsId { get; set; }
        public string? Brands { get; set; }
        public string? Model { get; set; }

        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? LineAmount { get; set; }

        public bool IsPresales { get; set; }
        public bool IsBold { get; set; }
       

        public decimal? UnitCost { get; set; }
        public decimal? LineCostTotal { get; set; }
        public decimal? UnitSalePrice { get; set; }
        public decimal? LineSaleTotal { get; set; }
        public decimal? MargenPorcentLine { get; set; }

        public long? PresalesAssignedId { get; set; }
        public string? PresalesAssignedTo { get; set; }

        public long? SystemId { get; set; }
        public string? SystemName { get; set; }

        public long? SuppliersId { get; set; }
        public string? SuppliersName { get; set; }

        public int? DeliveryDays { get; set; }

        public long? PmConditionId { get; set; }
        public string? PmConditionName { get; set; }
        public int? PmConditionDay { get; set; }
        public int? OrderMonthNo { get; set; }
    }
    public class SalesQuotationMarginDto
    {
        public long QuotationVerId { get; set; }

        public long? QuotationMarginVerId { get; set; }
        public long? MarginTypeId { get; set; }
        public string? MarginTypeName { get; set; }
        public decimal? MarginRate { get; set; }
    }
    public class SalesQuotationServCheckDto
    {
        public long QuotationVerId { get; set; }
        public long? ServiceCheckId { get; set; }

        public string? ServCheckTypeName { get; set; }
        public decimal? QuotationAmount { get; set; }
        public decimal? ScheduleAmount { get; set; }
        public decimal? DifferenceAmount { get; set; }
    }

    public class SalesQuotationLinePlanDto
    {
        public int TempLineNo { get; set; }
        public long? LinePlanId { get; set; }
        public List<SalesQuotationLinePlanLinDto> Lines { get; set; } = new();
    }
    public class SalesQuotationLinePlanLinDto
    {
        public int SeqNo { get; set; }
        public int MonthNo { get; set; }
        public int PaymentNo { get; set; }

        public decimal? PaymentPercent { get; set; } // 0.00 - 1.00
        public decimal? PaymentAmount { get; set; }
    }
    public class SalesQuotationEgressDto
    {
        public int MonthNo { get; set; }             // 0..11
        public decimal Amount { get; set; }          // Total por mes
        public List<SalesQuotationEgressLinDto> Lines { get; set; } = new();
    }

    public class SalesQuotationEgressLinDto
    {
        public int? TempLineNo { get; set; }
        public int LineNo { get; set; }
        public int MonthNo { get; set; }
        public decimal Amount { get; set; }
    }
}
