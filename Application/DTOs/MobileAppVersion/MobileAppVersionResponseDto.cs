namespace Application.DTOs.MobileAppVersion
{
    public class MobileAppVersionResponseDto
    {
        public string MinimumVersion { get; set; } = string.Empty;
        public int MinimumBuildNumber { get; set; }
        public string LatestVersion { get; set; } = string.Empty;
        public int LatestBuildNumber { get; set; }
        public bool ForceUpdate { get; set; }
        public string? AndroidUrl { get; set; }
        public string? Message { get; set; }
    }
}
