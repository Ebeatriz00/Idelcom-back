namespace Core.Options
{
    public class ApiPeruOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string EndpointPath { get; set; } = string.Empty;
        public Dictionary<string, string> Endpoints { get; set; } = new();
        public string ApiKey { get; set; } = string.Empty;
        public string ApiKeyHeaderName { get; set; } = "Authorization";
        public string ApiKeyHeaderValuePrefix { get; set; } = "Bearer";
        public int TimeoutSeconds { get; set; } = 15;
    }
}
