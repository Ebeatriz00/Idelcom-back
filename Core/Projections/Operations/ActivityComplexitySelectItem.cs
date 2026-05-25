namespace Core.Projections.Operations
{
    public class ActivityComplexitySelectItem
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal WeightFactor { get; set; }
    }
}
