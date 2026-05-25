using System.Text.Json.Serialization;

namespace Application.DTOs.OperationsTeamSsoma
{
    public enum SsomaAssignmentChangeType
    {
        Update = 1,
        Relocation = 2,
        Replacement = 3
    }

    public class ProcessSsomaAssignmentChangeDto
    {
        [JsonIgnore]
        public SsomaAssignmentChangeType ChangeType { get; set; }
        public long OperationsTeamSsomaId { get; set; }
        public long BusinessId { get; set; }
        public long SsomaProcessId { get; set; }
        public long? WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsPrimary { get; set; }
        public string? ReasonChange { get; set; }
        public int? ClientApprovalStatusId { get; set; }
        public DateTime? ClientApprovalDate { get; set; }
        public string? Comments { get; set; }
        public long UpdateUser { get; set; }
        public long? ReplacedAssignmentId { get; set; }
        public long SssomaMovementTypeId { get; set; }
        public DateTime MovementDate { get; set; }
        public long? FromSsomaProcessId { get; set; }
        public long? ToSsomaProcessId { get; set; }
        public string? Description { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}
