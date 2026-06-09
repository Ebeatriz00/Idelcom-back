namespace Application.DTOs.Ssoma.Clinics
{
    public class ClinicCreateDto
    {
        public long BusinessId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public long CreateUser { get; set; }
    }
}
