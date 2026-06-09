namespace Core.Entities.Ssoma.Clinics
{
    public class Clinic
    {
        public long ClinicId { get; set; }
        public long BusinessId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateUser { get; set; }
        public string Status { get; set; } = "1";
    }
}
