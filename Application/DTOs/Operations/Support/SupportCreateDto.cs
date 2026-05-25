namespace Application.DTOs.Operations.Support
{
    public class SupportCreateDto
    {
        public string? Provider { get; set; }
        public string? Service { get; set; }
        public string? Url { get; set; }
        public string? Access { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? SupportState { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Comments { get; set; }
        public string? Remarks { get; set; }
    }
}
