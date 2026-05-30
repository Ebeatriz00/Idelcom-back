using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Operations.Operations
{ 
    public class OperationsUpdateDto
    {
        public long OperationsId { get; set; }
        public long OpporId { get; set; }
        public long? QualitySupervisorId { get; set; }
        public long? ProjectManagerId { get; set; }
        public bool? RequeredSsoma { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? OperationsStatusId { get; set; }
        public string? Status { get; set; }
        public IFormFile? ClosurePdfFile { get; set; }
    }
}
