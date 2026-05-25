using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dashboard
{
    public class DashboardCommercialProbabilityDto
    {
        public long StateOpportunityId { get; set; }
        public int PorcentProgressPro { get; set; }
        public decimal TotalAmount { get; set; }
        public string StateDesc { get; set; }
        public string StateColor { get; set; }
    }


    public class DashboardCommercialEvolutionDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long StateOpportunityId { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DashboardCommercialClosingDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long StateOpportunityId { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DashboardCommercialClientDto
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long StateOpportunityId { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DashboardCommercialTotalsDto
    {
        public int TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public int WonQty { get; set; }
        public decimal WonAmount { get; set; }
        public decimal ConversionRate { get; set; }
    }
}
