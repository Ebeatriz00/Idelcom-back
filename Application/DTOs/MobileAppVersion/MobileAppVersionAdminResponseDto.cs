namespace Application.DTOs.MobileAppVersion
{
    public class MobileAppVersionAdminResponseDto : MobileAppVersionResponseDto
    {
        public long Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }
}
