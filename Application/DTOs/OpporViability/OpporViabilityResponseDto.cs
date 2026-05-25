using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OpporViability
{
    public class OpporViabilityResponseDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string OpporNum { get; set; } = default!;
        public string OpporDesc { get; set; } = default!;
        public long ClientsId { get; set; }
        public string ClientsName { get; set; }
        public long GeneralStatesId { get; set; }
        public string GeneralStatesName { get; set; }
        public string ColorState { get; set; }
        public long StateOpportunityId { get; set; }

        public bool? IsHiring { get; set; }
        public bool? IsReEvaluation { get; set; }

        public int ViabilityScore { get; set; }
        public int Compliance { get; set; }
        public int PartialCompliance { get; set; }
        public int NonCompliance { get; set; }

        public int? ContractMethod { get; set; }
        public string? ContractMethodDesc { get; set; }
        public int? RequiresIsos { get; set; }
        public string? RequiresIsosDesc { get; set; }

        public int Authority { get; set; }
        public string? AuthorityDesc { get; set; }

        public int Budget { get; set; }
        public string? BudgetDesc { get; set; }

        public int Need { get; set; }
        public string? NeedDesc { get; set; }

        public int Term { get; set; }
        public string? TermDesc { get; set; }

        public int CompanyExperience { get; set; }
        public string? CompanyExperienceDesc { get; set; }

        public int WorkerExperience { get; set; }
        public string? WorkerExperienceDesc { get; set; }

        public int StaffExperience { get; set; }
        public string? StaffExperienceDesc { get; set; }

        public int Ability { get; set; }
        public string? AbilityDesc { get; set; }

        public int Schedule { get; set; }
        public string? ScheduleDesc { get; set; }
        public int? BrandAproach { get; set; }
        public string? BrandAproachDesc { get; set; }
        public int? TechnicalChanges { get; set; }
        public string? TechnicalChangesDesc { get; set; }

        public string OporStatesName { get; set; }
        public string OporColorState { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}
