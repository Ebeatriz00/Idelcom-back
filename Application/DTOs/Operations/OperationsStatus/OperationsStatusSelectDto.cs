namespace Application.DTOs.Operations.OperationsStatus
{
    public class OperationsStatusSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
