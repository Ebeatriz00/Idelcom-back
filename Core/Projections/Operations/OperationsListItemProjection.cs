using SharedKernel;

namespace Core.Projections.Operations
{
    public class OperationsListItemProjection : BaseAuditableEntity
    {
        public long OperationsId { get; set; }
        public long BusinessId { get; set; }
        public long OpporId { get; set; }
        public long? QualitySupervisorId { get; set; }
        public string? QualitySupervisorName { get; set; }
        public long? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public bool? RequeredSsoma { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? OperationsStatusId { get; set; }
        public string? OperationStatusDesc { get; set; }
        public decimal? ProgressPercentage { get; set; }
        public string? StateColor { get; set; }
        public Guid? ClosurePdfFileUid { get; set; }
    }
}
