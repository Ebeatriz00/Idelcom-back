namespace Application.DTOs.Operations.WorkDayStatus
{
    public class WorkDayStatusSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
