using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesGetByIdDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public int TypeOppor { get; set; }
        public long ParentOpporId { get; set; }
        public long NegotiationStagesId { get; set; }
        public string OpporDesc { get; set; }
        public string OpporNumber { get; set; }
        public int OpporNumInternal { get; set; }
        public long ClientsId { get; set; }
        public long? ContactsId { get; set; }
        public long? BusinessLineId { get; set; }
        public long? StateOpporId { get; set; }
        public long? WorkerId { get; set; }
        public long? CurrencyId { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateFinish { get; set; }
        public Decimal? OpporAmount { get; set; }
        public DateTime? ConsultDate { get; set; }
        public DateTime? QuoDate { get; set; }
        public int? PorcentProgressPro { get; set; }

        public bool? IsAprovedViability { get; set; }
        public bool? IsPreOpportunity { get; set; }
        public string? DecisionManager { get; set; }

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

        public int? RequiresIsos { get; set; }
        public string? RequiresIsosDesc { get; set; }

        public int? ContractMethod { get; set; }
        public string? ContractMethodDesc { get; set; }
        public int? BrandAproach { get; set; }
        public string? BrandAproachDesc { get; set; }
        public int? TechnicalChanges { get; set; }
        public string? TechnicalChangesDesc { get; set; }
        public bool? IsHiring { get; set; }
        public bool? IsReEvaluation { get; set; }
        public bool? FollowupEnabled { get; set; }
        public int? FollowupEveryDay { get; set; }
        public long? StatePreSalesId { get; set; }
        public List<DeliverablesOpporDto>? Deliverables { get; set; }
        public List<DeliverablesOpporDto>? DeliverablesHiring { get; set; }
        public List<OpportFiletrackingDto>? HiringFiles { get; set; }

        public long? FlowTypeId { get; set; }
        public long? PmConditionId { get; set; }
    }
}
