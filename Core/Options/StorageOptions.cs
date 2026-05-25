namespace Core.Options
{
    public class StorageOptions
    {
        public string RootPath { get; set; } = string.Empty;
        public string FilesBaseUrl { get; set; } = string.Empty;
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    }
}
