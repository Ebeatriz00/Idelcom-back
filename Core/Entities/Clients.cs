using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Clients
    {
        public long ClientsId { get; set; }
        public long BusinessId { get; set; }
        public long DocumentTypeId { get; set; }
        public string Documents {  get; set; }
        public string ClientsName { get; set; }
        public string? ClientsCompany {  get; set; }
        public string ClientsAddress { get; set; }
        public string? ClientsPhone { get; set; }
        public long? WorkerId { get; set; }
        public long? DepartmentId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? ProcessTypeId { get; set; }
        public long? SectorId { get; set; }
        public long? LeadSourceId { get; set; }
        public long? LeadStatusId { get; set; }
        public long? LeadQualificationId { get; set; }
        public string? Website { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }

        public string? Sales { get; set; }
        public string? Sector { get; set; }
        public string? Departament { get; set; }
        public string? LeadStatus { get; set; }

        public bool IsOtherSeller { get; set; }

        public long? EventId { get; set; }
        public DateTime ChangeAt { get; set; }
        public string? ChangeUser { get; set; }
        public string? Description { get; set; }

    }
    public class ClientsDetailDto
    {
        public ClientsDetailHeaderDto Header { get; set; } = new();
        public List<ClientsDetailOpportunityDto> Pipeline { get; set; } = new();
        public List<ClientsDetailContactDto> Contacts { get; set; } = new();
        public List<ClientActivityTrendDto> ActivityTrend { get; set; } = new();
    }

     public class ClientsDetailHeaderDto
    {
        public long ClientsId { get; set; }
        public string ClientsName { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public string DepartamentName { get; set; } = string.Empty;
        public int OpenOppCount { get; set; }
        public decimal OpenOppAmount { get; set; }
        public decimal LtvTotalAmount { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public int TotalQuotes { get; set; }
        public decimal WinRate { get; set; }
    }

    public class ClientsDetailOpportunityDto
    {
        public string OpporId { get; set; }
        public string OpporDesc { get; set; } = string.Empty;
        public string StateDesc { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
        public DateTime? FinishDate { get; set; }
        public string Status { get; set; }
        public decimal OpportunityAmount { get; set; }
    }

    public class ClientsDetailContactDto
    {
        public long ContactsCrmId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
    }

    public class ClientActivityTrendDto
    {
        public string MonthName { get; set; }  
        public int Year { get; set; }         
        public int MonthNum { get; set; }     
        public int ActivityCount { get; set; } 
    }

}
