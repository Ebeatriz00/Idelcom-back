namespace Application.DTOs.Operations.OperationsWorkOrderStatus
{
    public class OperationsWorkOrderStatusSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
    }
}
