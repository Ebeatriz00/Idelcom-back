namespace Application.DTOs.MobileAppVersion
{
    public class MobileAppVersionUpsertDto
    {
        public string Platform { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string MinimumVersionName { get; set; } = string.Empty;
        public int MinimumBuildNumber { get; set; }
        public string LatestVersionName { get; set; } = string.Empty;
        public int LatestBuildNumber { get; set; }
        public bool ForceUpdate { get; set; }
        public string? StoreUrl { get; set; }
        public string? Message { get; set; }
    }
}
