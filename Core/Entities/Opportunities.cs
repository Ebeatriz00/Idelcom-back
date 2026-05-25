using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Opportunities
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public int? TypeOppor { get; set; }
        public string? TypeOpporDesc { get; set; }
        public long? ParentOpporId { get; set; }

        public long NegotiationStagesId { get; set; }
        public string OpporDesc { get; set; }
        public string? OpporNumber { get; set; }
        public int OpporNumInternal { get; set; }
        public long ClientsId { get; set; }
        public long? ContactsId { get; set; }
        public long? BusinessLineId { get; set; }
        public long? WorkerId { get; set; }
        public int? PorcentProgressPro { get; set; }
        public long? StateOpporId { get; set; }
        public long? StateOpporGenId { get; set; }
        public long? CurrencyId { get; set; }
        public Decimal? OpporAmount { get; set; }
        public int? PorcentProgress { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateFinish { get; set; }
        public DateTime? ConsultDate { get; set; }
        public DateTime? QuoDate { get; set; }
        public long? FlowTypeId { get; set; }
        public long? PmConditionId { get; set; }

        /*=====================RECORDATORIO========================*/

        public bool? FollowupEnabled { get; set; }
        public bool? FollowupSuspended { get; set; }
        public int? FollowupEveryDay { get; set; }
        public DateTime? FollowupNextAt { get; set; }

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

        public bool? IsAprovedViability { get; set; }
        public bool? IsPreOpportunity { get; set; }
        public long? IsOpporManager { get; set; }
        public string? DecisionManager { get; set; }

        /*===================== END VIABILIDAD========================*/

        public long UsersBy { get; set; }
        public string Status { get; set; }

        public long? ReasonRejectionId { get; set; }
        public string? ReasonRejection { get; set; }
        public int? ManualAdvance { get; set; }
        public int? ProcessNum { get; set; }


        public string NegotiationStagesDesc { get; set; }
        public string? ProjectName { get; set; }
        public string? ClientsName { get; set; }
        public string? SalesName { get; set; }
        public string? SalesPre { get; set; }
        public int? Tasks { get; set; }
        public string? StateOpporDesc { get; set; }
        public string? StateGeneral { get; set; }
        public string? ColorState { get; set; }
        public string? StateColor { get; set; }

        public string? StatePresales { get; set; }
        public string? colorStatePresales { get; set; }
        public string? ClientsDocument { get; set; }
        public string? ClientsAddress { get; set; }
        public string? ClientsSector { get; set; }
        public string? DepartmentName { get; set; }
        public string? ClientsPhone { get; set; }
        public string? ClientsWeb { get; set; }

        public string? DescLineBusiness { get; set; }
        public string? OpporAmountStr { get; set; }


        public string? ContactsName { get; set; }
        public string? ContactsJob { get; set; }
        public string? ContactsPhone { get; set; }
        public string? ContactsEmail { get; set; }
        public string? ContactsType { get; set; }

        public string? StateProject { get; set; }
        public string? StateStatusProject { get; set; }
        public int? NumPercPro { get; set; }
        public int? PorcentProgressAdv { get; set; }
        public string? PorcentProgressTxt { get; set; }

        public string? WorkerResp { get; set; }
        public DateTime? FinishDateProject { get; set; }
        public string? FinishDateProjectTxt { get; set; }

        public string? ProposalComment { get; set; }

        public string? WonComment { get; set; }
        public long? QuotationVerId { get; set; }
        public DateTime? CallDate {  get; set; }

        public int? DeliverablesCount { get; set; }
        public int? CommentsCount { get; set; }
        public int? unreadCommentsCount { get; set; }
        public List<OpportTask>? TasksList { get; set; }
        public List<OpportProjectTeam>? ProjectTeamList { get; set; }
        public List<OpportActivity>? ActivityList { get; set; }
        public List<OpportFiletracking>? FiletrackingList { get; set; }
        public List<HistoryOpportunityChanges> HistoryChanges { get; set; }
        public List<DeliverablesOppor>? Deliverables { get; set; }

        public List<DeliverablesOppor>? DeliverablesHiring { get; set; }
        public bool? IsHiring {  get; set; }
        public bool? IsReEvaluation { get; set; }
        public List<OpportFiletracking>? HiringFiles { get; set; }

        /*=====================COTIZACIÓN========================*/ 
        public bool? ProposalPresentated { get; set; }
        public IFormFile? ExcelFile { get; set; }
        public string? FileTitle {  get; set; }
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
        public string? ArchiveType { get; set; }


        public decimal? TotalAmount { get; set; }
        public string? CurrencyDesc { get; set; }

        /*=====================OBSERVACIONES========================*/
        public List<ObservationOppor>? Observations { get; set; }

        /*===================== VALIDACIONES ========================*/
        public int? ObsNotResolved { get; set; }
        public int? ObsNotApproved { get; set; }
        public int? ObsApproved { get; set; }
        public int? ObsNotDate { get; set; }

        // Flag preventa
        public int? ObsNotResolvedPre { get; set; }
        public int? ObsNotApprovedPre { get; set; }
        public int? ObsApprovedPre { get; set; }
        public int? ObsNotDatePre { get; set; }
        public int? ObsQuo { get; set; }
        public int? ObsQuoResolved { get; set; }
        public int? ExistQuo { get; set; }
        public int? GoesToPreSales { get; set; }
        public int? PreSalesDelivered { get; set; }
        // Flag contrataciones
        public int? ObsNotResolvedLic { get; set; }
        public int? ObsNotApprovedLic { get; set; }
        public int? ObsApprovedLic { get; set; }
        public int? ObsNotDateLic { get; set; }
        public int? LicDocDelivered { get; set; }
        public int? LicConsultDelivered { get; set; }
        public int? CanChangeStateByDelivery { get; set; }

        /*===================== RESULTADOS ========================*/
        public long? NegotationOutcomeId { get; set; }
        public int? TypeObsClientsId { get; set; }
        public string? ReasonObsClients { get; set; }
        public int? TypeObsEconomic { get; set; }

       
    }

    public class OpportTask
    {
        public string TasksToken { get; set; } = default!;
        public string PriorityToken { get; set; } = default!;
        public string? TitleTasks { get; set; }
        public string? DescTasks { get; set; }
        public string? PriorityColor { get; set; }
        public string? PriorityDesc { get; set; }
        public string? TasksResp { get; set; }
        public string? StatusTasks { get; set; }
        public int? StatusProgress { get; set; }
        public string? StateColor { get; set; }
        public DateTime? EndRegister { get; set; }
    }

    public class OpportProjectTeam
    {
        public string? TeamMember { get; set; }
    }

    public class OpportActivity
    {
        public string LinkToken { get; set; } = default!;
        public string? Activitys { get; set; }
        public string? MessageAddition { get; set; }
        public DateTime? DateActivity { get; set; }
        public string? workerName { get; set; }
        public string? ActivityState { get; set; }
        public string? ActivityStateColor { get; set; }

        public string? ActivityIcon { get; set; }
        public string? Activity { get; set; }
        public string? ActivityPriority { get; set; }
        public string? ActivityPriorityColor { get; set; }
    }

    public class OpportFiletracking
    {
        public string? LinkToken { get; set; } = default!;
        public string? ArchiveType { get; set; }
        public string? FileUrl { get; set; }
        public string? CommentFile { get; set; }
        public string? FileTitle { get; set; }
        public string? RelativePath { get; set; }
        public string? CodeOppor { get; set; }
        public DateTime? DateUpload { get; set; }
    }

    public class HistoryOpportunityChanges
    {
        public string? UsersName { get; set; }
        public string? History { get; set; }
        public DateTime? DateChange { get; set; }
    }

    public class DeliverablesOppor
    {
        public long? DeliverablesId { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public DateTime? DueDate { get; set; }

        public long? RequestId { get; set; }
        public string? DeliverablesName { get; set; }
        public string? State { get; set; }
        public string? TaskToken { get; set; }
        public long? TasksId { get; set; }      
        public string? TaskStateDesc { get; set; }
        public string? TaskStateColor { get; set; }
        public int? TypeDeliverable { get; set; }
        public int? NumPercPro { get; set; }


    }

    public class ObservationOppor
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
