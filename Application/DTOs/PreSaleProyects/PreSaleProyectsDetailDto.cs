using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PreSaleProyects
{

    public class PreSaleProyectsDetailDto
    {
        public long PreSaleProyectId { get; set; }
        public long BusinessId { get; set; }
        public string LinkToken { get; set; } = default!;
        public string Description { get; set; }
        public string ClientsName { get; set; }
        public string ResponsibleDescription { get; set; }
        public DateTime? EndDate { get; set; }

        public string ProyectNum { get; set; }

        public string ClientsSector { get; set; }
        public string PresupuestoEstimado { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string StatePreSaleDescription { get; set; }
        public string NumPercPro { get; set; }
        public string ClientsRuc { get; set; }
        public string ClientsAddress { get; set; }
        public string ClientsCity { get; set; }
        public string ClientsPhone { get; set; }
        public string ClientsWeb { get; set; }
        public string ContactName { get; set; }
        public string ContactJob { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactType { get; set; }
        public string CurrencyName { get; set; }
        public string? ReasonRejection { get; set; }

        public List<PreSaleProjectTaskDto>? TasksList { get; set; }
        public List<HistoryPreSaleProjectsChangesDto> HistoryChanges { get; set; }
        public List<PreSaleProjectActivityDto>? ActivityList { get; set; }
        public List<ProjectFiletrackingDto>? FiletrackingList { get; set; }
    }


        public class PreSaleProjectTaskDto
        {
            public string TasksToken { get; set; } = default!;
            public string PriorityToken { get; set; } = default!;
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

        public class HistoryPreSaleProjectsChangesDto
        {
            public string? UsersName { get; set; }
            public string? History { get; set; }
            public DateTime? DateChange { get; set; }
        }

        public class PreSaleProjectActivityDto
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


        public class ProjectFiletrackingDto
        {
            public string LinkToken { get; set; } = default!;
            public string? FileUrl { get; set; }
            public string? CommentFile { get; set; }
            public string? FileTitle { get; set; }
            public string? RelativePath { get; set; }
            public string? CodeProject { get; set; }
            public DateTime? DateUpload { get; set; }
            public string? ArchiveType { get; set; }
    }
    }

