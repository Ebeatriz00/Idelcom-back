using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesStateGetByIdDto
    {
        public string LinkToken { get; set; } = default!;
        public string? OpporNumber { get; set; }
        public long BusinessId { get; set; }
        public long? StateOpporId { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateFinish { get; set; }
        public long? ReasonRejectionId { get; set; }
        public string? ReasonRejection { get; set; }
        public string? ProposalComment { get; set; }
        public string? WonComment { get; set; }

        public List<DeliverablesOpporDto>? Deliverables { get; set; }

        public long? currencyId { get; set; }
        public  decimal? ExRate { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Utility { get; set; }

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

        public int? ContractMethod { get; set; }
        public string? ContractMethodDesc { get; set; }

        public int? BrandAproach { get; set; }
        public string? BrandAproachDesc { get; set; }
        public int? TechnicalChanges { get; set; }
        public string? TechnicalChangesDesc { get; set; }

        public int? RequiresIsos { get; set; }
        public string? RequiresIsosDesc { get; set; }

        public List<DeliverablesOpporDto>? DeliverablesHiring { get; set; }

        /*=====================COTIZACIÓN========================*/
        public bool? ProposalPresentated { get; set; }
        public IFormFile? ExcelFile { get; set; }

        /*=====================OBSERVACIONES========================*/
        public bool? IsHiring { get; set; }
        public bool? IsReEvaluation { get; set; }
        public List<ObservationOpporDto>? Observations { get; set; }
        /*===================== RESULTADOS ========================*/
        public long? NegotationOutcomeId { get; set; }
        public int? TypeObsClientsId { get; set; }
        public string? ReasonObsClients { get; set; }
        public int? TypeObsEconomic { get; set; }
        public long? StateOpporGenId { get; set; }

    }
    public class DeliverablesOpporByIdDto
    {
        public long? DeliverablesId { get; set; }
        public string? Comment { get; set; }
        public DateTime? DueDate { get; set; }

        public long? RequestId { get; set; }
        public string? DeliverablesName { get; set; }
        public string? State { get; set; }
        public long? TasksId { get; set; }
        public string TaskToken { get; set; }
        public string? TaskStateDesc { get; set; }
        public string? TaskStateColor { get; set; }
        public int? TypeDeliverable { get; set; }
    }
    public class ObservationOpporDto
    {
        public long? ObsId { get; set; }
        public long? OpporId { get; set; }
        public int? ObsSeverity { get; set; }
        public string? ObsSeverityDesc { get; set; }
        public string? ObsColor { get; set; }
        public string? ObsComment { get; set; }
        public DateTime? DueDate { get; set; }
        public long? DueSetBy { get; set; }

    }

}
