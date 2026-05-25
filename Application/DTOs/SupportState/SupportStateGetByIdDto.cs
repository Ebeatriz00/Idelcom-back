namespace Application.DTOs.SupportState
{
    public class SupportStateGetByIdDto
    {
        public int SupportStateId { get; set; }
        public long BusinessId { get; set; }
        public string? StatusDesc { get; set; }
        public string? StatusColor { get; set; }
    }
}
