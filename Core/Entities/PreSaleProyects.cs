using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PreSaleProyects
    {
        public long PreSaleProyectId { get; set; }
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string ProyectNum { get; set; }
        public int ProyectNumInt { get; set; }
        public long ClientsId { get; set; }
        public long ContactsCrmId { get; set; }
        public long? ResponsibleId { get; set; }
        public long SupervisorId { get; set; }
        public long SsomaId { get; set; }
        public long TecLeaderId { get; set; }
        public long OpportunityId { get; set; }
        public long? StatePreSaleId { get; set; }
        public long? ContractObservationsPending { get; set; }
        public long QuotationNumberId { get; set; }
        public long OrderNumberId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? QuoDate { get; set; }
        public int? Category { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public string ClientsDescription { get; set; }
        public string ContactsCrmDescription { get; set; }
        public string ResponsibleDescription { get; set; }
        public string SupervisorDescription { get; set; }
        public string SsomaDescription { get; set; }
        public string TecLeaderDescription { get; set; }
        public string OpportunityDescription { get; set; }
        public string StatePreSaleDescription { get; set; }
        public string StateColor { get; set; }
        public string QuotationNumberDescription { get; set; }
        public string OrderNumberDescription { get; set; }
        public int PreSaleProyectsCount { get; set; }
        public int? ProjectCategory { get; set; }
        public int? ContractTotalCount { get; set; }
        public int? CommercialTotalCount { get; set; }
        public int? ObsRevised { get; set; }
        public int? UnreadCommentsCount { get; set; }
        public string? OpportunityStateDesc { get; set; }
        public string? OpportunityStateColor { get; set; }
        public DateTime? FinishDate { get; set; }

        public long ProjectTeam {  get; set; }

        public string StateGeneralDesc { get; set; }
        public string StateGeneralColor { get; set; }
        public string? OpportunityNumber { get; set; }
        public string? SellerDescription { get; set; }
        public long SellerId {  get; set; }

        public int ObservationsCount { get; set; }
        public DateTime? CreateDate { get; set; }

        public string PresupuestoEstimado { get; set; }
        public string ClientsName { get; set; } 
        public DateTime CloseDate { get; set; }
        public string ContactName { get; set; }

        public string ClientsRuc { get; set; }
        public string ClientsAddress { get; set; }
        public string ClientsSector { get; set; }
        public string ClientsCity { get; set; }
        public string ClientsPhone { get; set; }
        public string ClientsWeb { get; set; }


        public string ContactJob { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactType { get; set; }
        public string CurrencyName { get; set; }
        public string? ReasonRejection { get; set; }
        public int? NumPercPro { get; set; }
        public int? ObsType { get; set; }
        public int? ObsSeverity { get; set; } 
        public List<string>? ObsReason { get; set; }
        public long? ObsStatusId { get; set; }
        public DateTime? ObsDueDate { get; set; }


        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CostTotal { get; set; }
        public string? CurrencyDesc { get; set; }

        public List<long>? AssignedWorkerId { get; set; } = new();
        public List<PreSaleProyectFile>? PreSaleProyectFiles { get; set; }
        public List<PreSaleProjectTask>? TasksList { get; set; }
        public List<HistoryPreSaleProjectsChanges> HistoryChanges { get; set; }
        public List<PreSaleProjectActivity>? ActivityList { get; set; }
        public List<ProjectFiletracking>? FiletrackingList { get; set; }
        public List<ProjectTeam>? ProjectTeamList { get; set; }

    }

    public class PreSaleProjectTask
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




    public class HistoryPreSaleProjectsChanges
    {
        public string? UsersName { get; set; }
        public string? History { get; set; }
        public DateTime? DateChange { get; set; }
    }


    public class PreSaleProjectActivity
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



    public class ProjectFiletracking
    {
        public string LinkToken { get; set; } = default!;
        public string? FileUrl { get; set; }
        public string? CommentFile { get; set; }
        public string? FileTitle { get; set; }
        public string? RelativePath { get; set; }
        public string? CodeProject { get; set; }
        public string? ArchiveType { get; set; }

        public DateTime? DateUpload { get; set; }
    }


    public class PreSaleProyectFile
    {
        public string FileTitle { get; set; }
        public string FileUrl { get; set; }
        public string RelativePath { get; set; }
        public string ArchiveType { get; set; } 
    }
 
}
