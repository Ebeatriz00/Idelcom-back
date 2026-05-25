namespace Application.DTOs.Operations.MeasurementUnit
{
    public class MeasurementUnitSelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Symbol { get; set; }
    }
}
