namespace Application.DTOs.Ssoma.Clinics
{
    public class ClinicResponseDto
    {
        public long ClinicId { get; set; }
        public long BusinessId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
