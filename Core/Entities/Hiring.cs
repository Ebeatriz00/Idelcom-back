using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Hiring
    {
        public long HiringId { get; set; }
        public string LinkToken { get; set; }
        public string TaskToken { get; set; }
        public long BusinessId { get; set; }
        public long OpporId { get; set; }
        public string? RequestNote { get; set; }
        public long? LicitatorId { get; set; }
        public long? UsersBy { get; set; }
        public string? Status { get; set; }

        public string? OpporNum { get; set; }
        public string? OpporDesc { get; set; }
        public string? ClientsName { get; set; }
        public string? HiringStatus { get; set; }
        public string? OpporStatus { get; set; }
        public string? HiringStatusColor { get; set; }
        public string? OpporStatusColor { get; set; }
        public int? ProcessType { get; set; }
        public long? LicStatusId { get; set; }
        public int? UnreadFilesCount { get; set; }
        public long? StateOpportunityId { get; set; }  
        public long? TasksId { get; set; }           
        public long? StateTaskId { get; set; }        
        public string? TaskDesc { get; set; }       
        public string? TaskColor { get; set; }
        public int? TypeDeliverable { get; set; }
        public long? ObsStatusId { get; set; }
        public long? StatePreSalesId { get; set; }
        public int? TypeObsClients { get; set; }
        public Boolean? AffectsQuotatation { get; set; }
        public int? TypeObsEconomic { get; set; }

        public List<DerivablesHiring>? DerivablesHirings { get; set; }
        public List<HiringFile>? HiringFiles { get; set; }
    }

    public class DerivablesHiring
    {
        public long? DeliverablesHiringId { get; set; }
        public string? NameHiring { get; set; }
        public string? CommentHiring { get; set; }
        public DateTime? DueDateHiring { get; set; }

        public long? RequestHiringId { get; set; }
        public string? DeliverablesNameHiring { get; set; }
        public string? State { get; set; }
    }


    public class HiringFile 
    {
        public string FileTitle { get; set; }
        public string FileUrl { get; set; }
        public string RelativePath { get; set; }
        public string ArchiveType { get; set; }
    }
}
