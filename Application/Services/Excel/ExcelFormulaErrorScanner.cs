using Application.DTOs.Quotation;
using System.IO.Compression;
using System.Xml.Linq;

namespace Application.Services.Excel
{
    public static class ExcelFormulaErrorScanner
    {
        private static readonly string[] FormulaErrorTokens =
        {
            "#REF!",
            "#NAME?",
            "#DIV/0!",
            "#VALUE!",
            "#N/A",
            "#NUM!",
            "#NULL!",
            "#GETTING_DATA"
        };

        public static List<QuotationExcelValidationError> Scan(Stream input)
        {
            if (input.CanSeek)
                input.Position = 0;

            using var copy = new MemoryStream();
            input.CopyTo(copy);
            copy.Position = 0;

            using var zip = new ZipArchive(copy, ZipArchiveMode.Read, leaveOpen: false);
            var sheetNames = BuildSheetNameMap(zip);
            var errors = new List<QuotationExcelValidationError>();

            ScanWorksheetFormulas(zip, sheetNames, errors);

            return errors;
        }

        private static void ScanWorksheetFormulas(
            ZipArchive zip,
            IReadOnlyDictionary<string, string> sheetNames,
            List<QuotationExcelValidationError> errors)
        {
            var sheetEntries = zip.Entries
                .Where(e => e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase)
                         && e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var entry in sheetEntries)
            {
                var sheet = sheetNames.TryGetValue(entry.FullName, out var name) ? name : entry.FullName;
                if (!IsRelevantImportSheet(sheet))
                    continue;

                var doc = LoadXml(entry);
                var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

                foreach (var cell in doc.Descendants(ns + "c"))
                {
                    var address = cell.Attribute("r")?.Value ?? "";
                    var formula = cell.Element(ns + "f")?.Value;
                    var value = cell.Element(ns + "v")?.Value;
                    var token = FindFormulaErrorToken(formula) ?? FindFormulaErrorToken(value);

                    if (token == null)
                        continue;

                    errors.Add(new QuotationExcelValidationError
                    {
                        Sheet = sheet,
                        Row = TryGetRow(address),
                        Column = TryGetColumn(address),
                        Message = string.IsNullOrWhiteSpace(address)
                            ? $"Formula con error {token} en la hoja '{sheet}'."
                            : $"Formula con error {token} en la hoja '{sheet}', celda {address}."
                    });
                }
            }
        }

        private static Dictionary<string, string> BuildSheetNameMap(ZipArchive zip)
        {
            var workbookEntry = zip.GetEntry("xl/workbook.xml");
            var relsEntry = zip.GetEntry("xl/_rels/workbook.xml.rels");
            if (workbookEntry == null || relsEntry == null)
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var workbook = LoadXml(workbookEntry);
            var rels = LoadXml(relsEntry);
            var workbookNs = workbook.Root?.Name.Namespace ?? XNamespace.None;
            var relNs = rels.Root?.Name.Namespace ?? XNamespace.None;
            var relById = rels.Descendants(relNs + "Relationship")
                .Select(r => new
                {
                    Id = r.Attribute("Id")?.Value,
                    Target = r.Attribute("Target")?.Value
                })
                .Where(r => !string.IsNullOrWhiteSpace(r.Id) && !string.IsNullOrWhiteSpace(r.Target))
                .ToDictionary(r => r.Id!, r => NormalizeWorkbookTarget(r.Target!), StringComparer.OrdinalIgnoreCase);

            XNamespace relAttrNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sheet in workbook.Descendants(workbookNs + "sheet"))
            {
                var name = sheet.Attribute("name")?.Value;
                var relId = sheet.Attribute(relAttrNs + "id")?.Value;

                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(relId) ||
                    !relById.TryGetValue(relId, out var path))
                {
                    continue;
                }

                result[path] = name;
            }

            return result;
        }

        private static string NormalizeWorkbookTarget(string target)
        {
            target = target.Replace('\\', '/').TrimStart('/');
            return target.StartsWith("xl/", StringComparison.OrdinalIgnoreCase)
                ? target
                : $"xl/{target}";
        }

        private static XDocument LoadXml(ZipArchiveEntry entry)
        {
            using var stream = entry.Open();
            return XDocument.Load(stream);
        }

        private static string? FindFormulaErrorToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return FormulaErrorTokens.FirstOrDefault(token =>
                value.Contains(token, StringComparison.OrdinalIgnoreCase));
        }

        private static int? TryGetRow(string address)
        {
            var digits = new string(address.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var row) ? row : null;
        }

        private static string? TryGetColumn(string address)
        {
            var letters = new string(address.Where(char.IsLetter).ToArray());
            return string.IsNullOrWhiteSpace(letters) ? null : letters;
        }

        private static bool IsRelevantImportSheet(string sheet)
        {
            var normalized = sheet.Trim().ToUpperInvariant();
            return normalized == "COTIZACION"
                || normalized == "COTIZACIÓN"
                || normalized == "COTIZACIÃ“N"
                || normalized == "DATOS COMERCIALES";
        }
    }
}
