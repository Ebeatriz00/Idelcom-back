namespace Application.DTOs.Operations.AssignmentStatus
{
    public class AssignmentStatusSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
    }
}
