using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Application.Services.Excel
{
    public static class ExcelSanitizer
    {
        public static Stream SanitizeBrokenRefs(Stream input)
        {
            if (input.CanSeek) input.Position = 0;

            var output = new MemoryStream();
            input.CopyTo(output);
            output.Position = 0;

            using (var zip = new ZipArchive(output, ZipArchiveMode.Update, leaveOpen: true))
            {
                FixWorkbookDefinedNames(zip);
                FixWorksheetFormulas(zip);
            }

            output.Position = 0;
            return output;
        }

        private static bool IsUnparseableFormula(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;

            // #REF en inglés y español
            if (s.Contains("#REF", StringComparison.OrdinalIgnoreCase) ||
                s.Contains("#¡REF", StringComparison.OrdinalIgnoreCase))
                return true;

            // Referencias externas / índices tipo [90]Sheet!A1
            // (ClosedXML suele morir con esto)
            if (s.Contains('[') || s.Contains(']'))
                return true;

            return false;
        }

        private static void FixWorkbookDefinedNames(ZipArchive zip)
        {
            var entry = zip.GetEntry("xl/workbook.xml");
            if (entry is null) return;

            XDocument doc;
            using (var s = entry.Open())
                doc = XDocument.Load(s);

            XNamespace ns = doc.Root?.Name.Namespace ?? XNamespace.None;

            var definedNames = doc.Descendants(ns + "definedName").ToList();
            var toRemove = definedNames
                .Where(dn => IsUnparseableFormula(dn.Value) ||
                             IsUnparseableFormula(dn.Attribute("refersTo")?.Value))
                .ToList();

            if (!toRemove.Any()) return;

            foreach (var dn in toRemove)
                dn.Remove();

            RewriteEntry(entry, doc);
        }

        private static void FixWorksheetFormulas(ZipArchive zip)
        {
            var sheetEntries = zip.Entries
                .Where(e => e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase)
                         && e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var entry in sheetEntries)
            {
                XDocument doc;
                using (var s = entry.Open())
                    doc = XDocument.Load(s);

                XNamespace ns = doc.Root?.Name.Namespace ?? XNamespace.None;

                // <f>...</f> fórmulas
                var formulaNodes = doc.Descendants(ns + "f").ToList();

                bool changed = false;

                foreach (var f in formulaNodes)
                {
                    var formulaText = f.Value;

                    if (IsUnparseableFormula(formulaText))
                    {
                        // Opción segura: reemplazar por fórmula válida (sin "=" en OOXML)
                        // Si tu objetivo es solo abrir el archivo y leer valores, esto evita que ClosedXML muera.
                        f.Value = "0";
                        changed = true;
                    }
                }

                if (changed)
                    RewriteEntry(entry, doc);
            }
        }

        private static void RewriteEntry(ZipArchiveEntry entry, XDocument doc)
        {
            using (var s = entry.Open())
            {
                s.SetLength(0);
                doc.Save(s);
            }
        }
    }
}