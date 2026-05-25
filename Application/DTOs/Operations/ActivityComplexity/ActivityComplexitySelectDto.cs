namespace Application.DTOs.Operations.ActivityComplexity
{
    public class ActivityComplexitySelectDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal WeightFactor { get; set; }
    }
}
