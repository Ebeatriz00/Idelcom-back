namespace Application.DTOs.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressCreateDto
    {
        public long ActivityId { get; set; }
        public decimal ReportedQuantity { get; set; }
        public DateTime ReportedDate { get; set; }
        public string? Observations { get; set; }
    }
}
