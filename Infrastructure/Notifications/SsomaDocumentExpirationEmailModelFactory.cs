using Core.Entities.Email;

namespace Infrastructure.Notifications
{
    public static class SsomaDocumentExpirationEmailModelFactory
    {
        private const int DetailedWorkersLimit = 10;

        private static readonly IReadOnlyDictionary<string, int> SeverityOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["HOY"] = 0,
            ["D1"] = 1,
            ["D3"] = 2,
            ["D7"] = 3,
            ["D15"] = 4,
            ["D30"] = 5,
            ["D60"] = 6
        };

        public static SsomaDocumentExpirationEmailModel Create(IReadOnlyCollection<SsomaDocumentExpirationPayloadItem>? payload)
        {
            if (payload is null || payload.Count == 0)
                throw new InvalidOperationException("Payload vacío para alerta SSOMA de documentos por vencer.");

            var normalized = payload.Select((item, index) => Normalize(item, index)).ToList();

            var workers = normalized
                .GroupBy(item => item.WorkerName, StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    var documents = group
                        .OrderBy(item => GetSeverityRank(item.Code))
                        .ThenBy(item => item.ExpiryDate)
                        .ThenBy(item => item.DocumentName)
                        .Select(item => new SsomaDocumentExpirationDocumentItem
                        {
                            Code = item.Code,
                            DocumentName = item.DocumentName,
                            ExpiryDate = item.ExpiryDate,
                            Scope = item.Scope
                        })
                        .ToList();

                    return new SsomaDocumentExpirationWorkerGroup
                    {
                        WorkerName = group.Key,
                        HighestPriorityCode = documents.First().Code,
                        Documents = documents
                    };
                })
                .OrderBy(group => GetSeverityRank(group.HighestPriorityCode))
                .ThenBy(group => group.WorkerName)
                .ToList();

            return new SsomaDocumentExpirationEmailModel
            {
                Totals = new SsomaDocumentExpirationTotals
                {
                    TotalWorkers = workers.Count,
                    TotalDocuments = normalized.Count,
                    DueTodayDocuments = normalized.Count(item => IsDueToday(item.Code)),
                    UpcomingDocuments = normalized.Count(item => !IsDueToday(item.Code))
                },
                Workers = workers,
                SummaryMode = workers.Count > DetailedWorkersLimit
            };
        }

        private static SsomaDocumentExpirationPayloadItem Normalize(SsomaDocumentExpirationPayloadItem item, int index)
        {
            if (item is null)
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: item nulo.");

            var code = item.Code?.Trim().ToUpperInvariant() ?? "";
            var documentName = item.DocumentName?.Trim() ?? "";
            var workerName = item.WorkerName?.Trim() ?? "";
            var scope = item.Scope?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: Code vacío.");

            if (!SeverityOrder.ContainsKey(code))
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: Code no soportado '{code}'.");

            if (string.IsNullOrWhiteSpace(documentName))
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: DocumentName vacío.");

            if (string.IsNullOrWhiteSpace(workerName))
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: WorkerName vacío.");

            if (item.ExpiryDate == default)
                throw new InvalidOperationException($"Payload SSOMA inválido en posición {index}: ExpiryDate inválido.");

            return new SsomaDocumentExpirationPayloadItem
            {
                Code = code,
                DocumentName = documentName,
                WorkerName = workerName,
                ExpiryDate = item.ExpiryDate,
                Scope = scope
            };
        }

        private static int GetSeverityRank(string code)
            => SeverityOrder.TryGetValue(code, out var rank) ? rank : int.MaxValue;

        private static bool IsDueToday(string code)
            => string.Equals(code, "HOY", StringComparison.OrdinalIgnoreCase);
    }
}
