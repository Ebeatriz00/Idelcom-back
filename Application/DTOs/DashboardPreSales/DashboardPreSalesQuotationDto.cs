using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DashboardPreSales
{
    public class DashboardPreSalesQuotationDto
    {
        public int Quantity { get; set; }
    }

    public class DashboardPreSalesStateDto
    {
        public long StatePreSaleId { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }

    public class DashboardPreSalesCombinedDto
    {
        public int QuarterNum { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }
    public class DashboardPreSalesByEngineerDto
    {
        public string Responsible { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSalesMatrizDto
    {
        public long WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int MonthNum { get; set; }
        public decimal WonAmount { get; set; }
        public decimal GeneralAmount { get; set; }
        public int TotalQuotations { get; set; }
    }

    public class DashboardPreSalesCollaboratorDto
    {
        public string CollaboratorName { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSalesIntegratorsDto
    {
        public string Integrators { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSaleByEngineerDetailsDto
    {
        public string Responsible { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

    public class DashboardPreSalesIntegratorsDetailsDto
    {
        public string Integrators { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

    public class DashboardPreSalesCollaboratorDetailsDto
    {
        public string Collaborators { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

        public class DashboardPreSalesByCategoryDto
        {
        public string CategoryName { get; set; }
        public int ProjectQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal WonAmount { get; set; }
    }
}
