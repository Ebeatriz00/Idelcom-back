namespace Application.DTOs.Operations.Support
{
    public class SupportResponseDto
    {
        public long SupportId { get; set; }
        public long BusinessId { get; set; }
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
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
    }
}
