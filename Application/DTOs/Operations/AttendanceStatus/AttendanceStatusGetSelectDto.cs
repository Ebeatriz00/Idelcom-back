namespace Application.DTOs.Operations.AttendanceStatus
{
    public class AttendanceStatusGetSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
    }
}
