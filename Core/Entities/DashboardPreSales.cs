using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DashboardPreSalesQuotation
    { 
        public int Quantity { get; set; }
    }

    public class DashboardPreSalesState
    {
        public long StatePreSaleId { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }

    public class DashboardPreSalesCombined
    {
        public int QuarterNum { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }

    public class DashboardPreSalesByEngineer
    {
        public string Responsible { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSaleByEngineerDetails
    {
        public string Responsible { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

    public class DashboardPreSalesMatriz
    {
        public long WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int MonthNum { get; set; }
        public decimal WonAmount { get; set; }
        public decimal GeneralAmount { get; set; }
        public int TotalQuotations { get; set; }

    }

    public class DashboardPreSalesCollaborator
    {
        public string CollaboratorName { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSalesCollaboratorDetails
    {
        public string Collaborators{ get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

    public class DashboardPreSalesIntegrators
    {
        public string Integrators { get; set; }
        public int TotalVersions { get; set; }
        public decimal GeneralAmount { get; set; }
        public decimal ClosedAmount { get; set; }
    }

    public class DashboardPreSalesIntegratorsDetails
    {
        public string Integrators { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public decimal GeneralAmount { get; set; }
        public int Category { get; set; }
    }

    public class DashboardPreSalesByCategory
    {
        public string CategoryName { get; set; }
        public int ProjectQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal WonAmount { get; set; }
    }
}
