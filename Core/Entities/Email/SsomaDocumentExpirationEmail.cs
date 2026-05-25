using System.Text.Json.Serialization;

namespace Core.Entities.Email
{
    public sealed class SsomaDocumentExpirationPayloadItem
    {
        public string Code { get; set; } = "";
        public string DocumentName { get; set; } = "";
        public string WorkerName { get; set; } = "";
        public DateOnly ExpiryDate { get; set; }
        public string Scope { get; set; } = "";

        [JsonPropertyName("doc")]
        public string? DocumentNameAlias
        {
            get => DocumentName;
            set => DocumentName = value ?? "";
        }

        [JsonPropertyName("worker")]
        public string? WorkerNameAlias
        {
            get => WorkerName;
            set => WorkerName = value ?? "";
        }

        [JsonPropertyName("expiry")]
        public DateOnly? ExpiryDateAlias
        {
            get => ExpiryDate == default ? null : ExpiryDate;
            set => ExpiryDate = value ?? default;
        }
    }

    public sealed class SsomaDocumentExpirationEmailModel
    {
        public SsomaDocumentExpirationTotals Totals { get; init; } = new();
        public List<SsomaDocumentExpirationWorkerGroup> Workers { get; init; } = new();
        public bool SummaryMode { get; init; }
        public bool ShowDetailedView => !SummaryMode;
        public DateOnly GeneratedDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public sealed class SsomaDocumentExpirationTotals
    {
        public int TotalWorkers { get; init; }
        public int TotalDocuments { get; init; }
        public int DueTodayDocuments { get; init; }
        public int UpcomingDocuments { get; init; }
    }

    public sealed class SsomaDocumentExpirationWorkerGroup
    {
        public string WorkerName { get; init; } = "";
        public string HighestPriorityCode { get; init; } = "";
        public List<SsomaDocumentExpirationDocumentItem> Documents { get; init; } = new();
        public int TotalDocuments => Documents.Count;
    }

    public sealed class SsomaDocumentExpirationDocumentItem
    {
        public string Code { get; init; } = "";
        public string DocumentName { get; init; } = "";
        public DateOnly ExpiryDate { get; init; }
        public string Scope { get; init; } = "";
    }
}
