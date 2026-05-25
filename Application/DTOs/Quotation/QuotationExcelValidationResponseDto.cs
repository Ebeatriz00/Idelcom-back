namespace Application.DTOs.Quotation
{
    public class QuotationExcelValidationResponseDto
    {
        public bool IsValid { get; set; }
        public string DetectedVariant { get; set; } = "A";
        public List<QuotationExcelValidationError> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class QuotationExcelValidationError
    {
        public string Sheet { get; set; } = "";
        public int? Row { get; set; }
        public string? Column { get; set; }
        public string Message { get; set; } = "";
    }
}
