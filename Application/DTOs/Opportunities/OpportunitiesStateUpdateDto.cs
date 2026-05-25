using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesStateUpdateDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long? StateOpporId { get; set; }
        public long? ReasonRejectionId { get; set; }
        public string? ReasonRejection { get; set; }
        public string? ProposalComment { get; set; }
        public long? PmConditionId { get; set; }
        public long UsersBy { get; set; }

        /*=====================VIABILIDAD========================*/
        public int? ViabilityScore { get; set; }
        public int? Compliance { get; set; }
        public int? PartialCompliance { get; set; }
        public int? NonCompliance { get; set; }
        public int? Authority { get; set; }
        public string? AuthorityDesc { get; set; }
        public int? Budget { get; set; }
        public string? BudgetDesc { get; set; }
        public int? Need { get; set; }
        public string? NeedDesc { get; set; }
        public int? Term { get; set; }
        public string? TermDesc { get; set; }
        public int? ContractMethod { get; set; }
        public string? ContractMethodDesc { get; set; }
        public int? RequiresIsos { get; set; }
        public string? RequiresIsosDesc { get; set; }
        public int? CompanyExperience { get; set; }
        public string? CompanyExperienceDesc { get; set; }
        public int? WorkerExperience { get; set; }
        public string? WorkerExperienceDesc { get; set; }
        public int? StaffExperience { get; set; }
        public string? StaffExperienceDesc { get; set; }
        public int? Ability { get; set; }
        public string? AbilityDesc { get; set; }
        public int? Shedule { get; set; }
        public string? SheduleDesc { get; set; }

        public int? BrandAproach { get; set; }
        public string? BrandAproachDesc { get; set; }
        public int? TechnicalChanges { get; set; }
        public string? TechnicalChangesDesc { get; set; }

        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public string? WonComment { get; set; }
        public long? QuotationVerId { get; set; }
        public DateTime? CallDate { get; set; }
        public bool? FollowupEnabled { get; set; }
        public long? StateOpporGenId { get; set; }

        /*===================== DELIVERABLES========================*/

        public List<DeliverablesOpporDto>? Deliverables { get; set; }
        public bool? IsReEvaluation { get; set; }
        public List<DeliverablesOpporDto>? DeliverablesHiring { get; set; }
        /*=====================COTIZACIÓN========================*/
        
        public bool? ProposalPresentated { get; set; }
        public IFormFile? ExcelFile { get; set; }
        public string? FileTitle { get; set; }
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
        public string? ArchiveType { get; set; }


        /*===================== OBSERVATIONS========================*/
        public List<ObservationOpporDto>? Observations { get; set; }

        /*===================== RESULTADOS ========================*/
        public long? NegotationOutcomeId { get; set; }
        public int? TypeObsClientsId { get; set; }
        public string? ReasonObsClients { get; set; }
        public int? TypeObsEconomic { get; set; }

    }
    public class DeliverablesOpporDto
    {
        public long? DeliverablesId { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public DateTime? DueDate { get; set; }
        public long? TasksId { get; set; }
        public string? TaskStateDesc { get; set; }
        public string? TaskStateColor { get; set; }
        public string? TaskToken { get; set; }
        public int? NumPercPro { get; set; }
        public int? TypeDeliverable { get; set; }
    }
}
