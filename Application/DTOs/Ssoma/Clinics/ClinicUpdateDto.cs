namespace Application.DTOs.Ssoma.Clinics
{
    public class ClinicUpdateDto
    {
        public long ClinicId { get; set; }
        public long BusinessId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public long UpdateUser { get; set; }
    }
}
