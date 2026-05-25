using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesDetailDto
    {

        public long BusinessId { get; set; }
        public string LinkToken { get; set; } = default!;
        public string OpporNumber { get; set; }
        public string OpporDesc { get; set; }
        public string? ClientsDocument { get; set; }
        public string? ClientsName { get; set; }
        public string? ClientsAddress { get; set; }
        public string? ClientsSector { get; set; }
        public string? DepartmentName { get; set; }
        public string? ClientsPhone { get; set; }
        public string? ClientsWeb { get; set; }
        public string? SalesName { get; set; }
        public DateTime? DateFinish { get; set; }
        public string? StateStatusProject { get; set; }
        public int? NumPercPro { get; set; }
        public string? DescLineBusiness { get; set; }
        public int? PorcentProgressPro { get; set; }
        public string? OpporAmountStr { get; set; }
        public DateTime? DateRegister { get; set; }

        public string? ContactsName { get; set; }
        public string? ContactsJob { get; set; }
        public string? ContactsPhone { get; set; }
        public string? ContactsEmail { get; set; }
        public string? ContactsType { get; set; }

        public string? StateProject { get; set; }
        public int? PorcentProgressAdv { get; set; }
        public string? PorcentProgressTxt { get; set; }

        public string? WorkerResp { get; set; }
        public DateTime? FinishDateProject { get; set; }

        public string? FinishDateProjectTxt { get; set; }
        public string? ReasonRejection { get; set; }

        public List<OpportTaskDto>? TasksList { get; set; }
        public List<OpportProjectTeamDto>? ProjectTeamList { get; set; }
        public List<OpportActivityDto>? ActivityList { get; set; }
        public List<OpportFiletrackingDto>? FiletrackingList { get; set; }
        public List<HistoryOpportunityChangesDto> HistoryChanges { get; set; }


    }

    public class OpportTaskDto
    {
        public string TasksToken { get; set; } = default!;
        public string PriorityToken { get; set; } = default!;
        public long TasksId { get; set; }
        public string? TitleTasks { get; set; }
        public string? DescTasks { get; set; }
        public string? PriorityColor { get; set; }
        public string? PriorityDesc { get; set; }
        public string? TasksResp { get; set; }
        public int? StatusProgress { get; set; }
        public string? StatusTasks { get; set; }
        public string? StateColor { get; set; }
        public DateTime? EndRegister { get; set; }
    }

    public class OpportProjectTeamDto
    {
        public string? TeamMember { get; set; }
    }

    public class OpportActivityDto
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


    public class OpportFiletrackingDto
    {
        public string? LinkToken { get; set; } = default!;
        public string? ArchiveType { get; set; }
        public string? FileTitle { get; set; }
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
        public string? CommentFile { get; set; }
        public string? CodeOppor { get; set; }
        public DateTime? DateUpload { get; set; }
    }
    public class HistoryOpportunityChangesDto
    {
        public string? UsersName { get; set; }
        public string? History { get; set; }
        public DateTime? DateChange { get; set; }
    }

    public class AttachHiringFilesDto
    {
        public long BusinessId { get; set; }
        public long OpporId { get; set; }
        public long UpdateUser { get; set; }
        public List<OpportFiletrackingDto> Files { get; set; } = new();
    }

}
