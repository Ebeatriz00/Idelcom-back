using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Ssoma
{
    public class SsomaPersonnelOperationsListItem
    {
        public long PersonnelOperationsId {  get; set; }
        public string PersonnelFullName { get; set; } = null!;
        public int GeneralPercentage { get; set; }
        public string HomologationStatus { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class SsomaPersonnelOperationsItem
    {
        public long PersonnelOperationsId { get; set; }
        public string PersonnelFullName { get; set; } = null!;
        public string PersonnelDocument {  get; set; } = null!;
        public string PersonnelPosittion { get; set; } = null!;
        public string Status { get; set; } = null!;

        public List<SsomaPersonnelHomologationGeneralItem> PersonnelHomologationGeneralItems { get; set; } =
            new List<SsomaPersonnelHomologationGeneralItem>();
        public List<SsomaPersonnelHomologationOperationsItem> PersonnelHomologationOperationsItem  { get; set; } = 
            new List<SsomaPersonnelHomologationOperationsItem>();
        public List<SsomaPersonnelHomologationsSummaryItem> PersonnelHomologationSummaryItem { get; set; } =
            new List<SsomaPersonnelHomologationsSummaryItem>();
    }

    public class SsomaPersonnelOperationsHeaderRow
    {
        public long PersonnelOperationsId { get; set; }
        public string PersonnelFullName { get; set; } = null!;
        public string PersonnelDocument { get; set; } = null!;
        public string PersonnelPosittion { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class SsomaPersonnelHomologationGeneralItem
    {
        public string Requeriment { get; set; } = null!;
        public long RequirementId { get; set; } 
        public long HomologationPersonnelId { get; set; }
        public string FileName { get; set; } = null!;
        public string ValidationStatus { get; set; } = null!;
        public string FileExpiration { get; set; } = null!;
        public string FileReview { get; set; } = null!;
        public string FileUrl { get; set; } = null!;

    }
    public class SsomaPersonnelHomologationOperationsItem
    {
        public long OperationsId { get; set; }
        public string OperationsName { get; set; } = null!;
        public string Requeriment { get; set; } = null!;
        public long RequirementId { get; set; }
        public long HomologationPersonnelId { get; set; }
        public string FileName { get; set; } = null!;
        public string ValidationStatus { get; set; } = null!;
        public string FileExpiration { get; set; } = null!;
        public string FileReview { get; set; } = null!;
        public string FileUrl { get; set; } = null!;

    }

    public class SsomaPersonnelHomologationsSummaryItem
    {
        public int ActiveProject { get; set; }
        public int CurrentDocuments {  get; set; }
        public int TotalDocuments { get; set; }
        public string SummaryCurrentDocuments { get; set; } = null!;
        public int Pendings {  get; set; }
        public int Observations { get; set; }
        public int Expired {  get; set; }
        public int ToExpired { get; set; }
        public int GeneralShortages { get; set; }
        public int OperationsShortages { get; set; }
        public int Shortages { get; set; }
        public int GeneralPercentage { get; set; }
    }

}
