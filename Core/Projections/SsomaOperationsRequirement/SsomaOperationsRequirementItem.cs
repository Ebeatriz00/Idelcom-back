using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.SsomaOperationsRequirement
{
    public class SsomaOperationsRequirementItem
    {

        public long OperationsRequirementId { get; set; }
        public int RequirementId { get; set; }
        public string RequirementName { get; set; } = null!;
        public string RequirementDescription { get; set;} = null!;
        public bool IsMandatory { get; set; }
        public bool RequiresFile { get; set; }
        public bool RequiresExpiration { get; set;} 
        public int MaxFileSize { get; set; }
        public string AllowedExtensions { get; set; } = null!;

    }

    public class SsomaOperationsRequirementByWorkerItem
    {

        public long OperationsRequirementId { get; set; }
        public int RequirementId { get; set; }
        public string RequirementName { get; set; } = null!;
        public int Duration { get; set; }
        public string RequirementDescription { get; set; } = null!;
        public bool IsMandatory { get; set; }
        public bool RequiresFile { get; set; }
        public bool RequiresExpiration { get; set; }
        public int MaxFileSize { get; set; }
        public string AllowedExtensions { get; set; } = null!;
        public int AllowInternalReuse { get; set; }

        public int? InternalDocumentId { get; set; }
        public string? InternalFileName { get; set; }
        public string? InternalFileUrl { get; set; }
        public string ? InternalFilePath { get; set; }
        public DateTime? InternalIssueDate { get; set; }
        public DateTime? InternalExpirationDate { get; set; }
        public int? InternalValidationStatusId { get; set; }
        public DateTime? InternalReviewDate { get; set; }
        public string? InternalObservation {  get; set; }

        public int? OperationDocumentId { get; set; }
        public string? OperationFileName { get; set; }
        public string? OperationFileUrl { get; set; }
        public string? OperationFilePath { get; set; }
        public DateTime? OperationIssueDate { get; set; }
        public DateTime? OperationlExpirationDate { get; set; }
        public int? OperationValidationStatusId { get; set; }
        public DateTime? OperationReviewDate { get; set; }
        public string? OperationObservation { get; set; }

        public string? SourceType { get; set; }
        public int? SourceDocumentId { get; set; }
        public string? RequirementState { get; set; }
        public bool? IsSatisfied {  get; set; }
        public bool? NeedsUpload { get; set; }

    }

    public class ValidateSsomaOperationsRequirementByWorkerItem
    {
        public int? IsReady { get; set; }
        public int? TotalMandatory { get; set; }
        public int? TotalCoveredMandatory { get; set; }
        public int? TotalMissingMandatory { get; set; }
    }

    public class SsomaOperationsRequirementMissingByWorkerItem
    {

        public int RequirementId { get; set; }
        public string RequirementName { get; set; } = null!;
        public string RequirementDescription { get; set; } = null!;

    }
}
