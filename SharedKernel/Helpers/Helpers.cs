using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedKernel.Helpers
{
    public class Helpers
    {
        public static MemoryStream RemoveBrokenDefinedNames(Stream input)
        {
            var ms = new MemoryStream();
            input.CopyTo(ms);
            ms.Position = 0;

            using (var doc = SpreadsheetDocument.Open(ms, true))
            {
                var wbPart = doc.WorkbookPart;
                if (wbPart?.Workbook?.DefinedNames == null)
                    return ms;

                var broken = wbPart.Workbook.DefinedNames
                    .Elements<DefinedName>()
                    .Where(n => (n.Text ?? "").Contains("#REF!"))
                    .ToList();

                foreach (var dn in broken)
                    dn.Remove();

                wbPart.Workbook.Save();
            }

            ms.Position = 0;
            return ms;
        }


        public static int? ExtractNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var match = System.Text.RegularExpressions.Regex.Match(value, @"\d+");
            return match.Success ? int.Parse(match.Value) : null;
        }


        public static decimal? SafeDecimal(IXLCell cell)
        {
            if (cell == null) return null;

            try
            {
                if (cell.TryGetValue<double>(out var d))
                    return Convert.ToDecimal(d);

                var s = cell.GetFormattedString()?.Trim();
                if (string.IsNullOrWhiteSpace(s)) return null;

                s = s.Replace("$", "").Replace(",", "").Trim();
                if (s == "-" || s == "$-") return null;

                return decimal.TryParse(s, out var v) ? v : null;
            }
            catch
            {
                return null;
            }
        }



        public static decimal? SafePercent(IXLCell cell)
        {
            if (cell == null) return null;

            try
            {
                // 1) Excel numérico: puede venir como 0.1873 (formato %) o 18.73 (ya en puntos)
                if (cell.TryGetValue<double>(out var d))
                {
                    var v = Convert.ToDecimal(d);

                    // heurística: si está entre 0 y 1, es fracción => convertir a puntos
                    if (v >= 0m && v <= 1m) return v * 100m;

                   
                    return v;
                }

                // 2) Texto formateado tipo "18.73%"
                var s = cell.GetFormattedString()?.Trim();
                if (string.IsNullOrWhiteSpace(s)) return null;

                s = s.Replace("%", "").Trim();

                // limpiar separadores 
                s = s.Replace(",", "");

                if (!decimal.TryParse(s, out var p)) return null;

                return p; 
            }
            catch
            {
                return null;
            }
        }

        public static string Normalize(string s)
        {
            return s
                .Replace(":", "")
                .Replace("%", "")
                .Replace(":", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Replace("%", " ")
                .Trim()
                .ToUpperInvariant();
        }


        public static IXLCell? FindLabelCell(IXLWorksheet ws, params string[] labels)
        {
            var normalized = labels.Select(Normalize).ToList();

            IXLCell? best = null;
            int bestScore = 0;

            foreach (var c in ws.CellsUsed(XLCellsUsedOptions.All))
            {
                // ✅ usar texto mostrado (incluye resultado de fórmulas en muchos casos)
                var raw = c.GetFormattedString();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = c.GetString();

                var cellText = Normalize(raw);
                if (string.IsNullOrWhiteSpace(cellText)) continue;

                foreach (var lbl in normalized)
                {
                    int score = 0;

                    // 🔥 Queremos priorizar "DESCUENTO (20%)" sobre "DESCUENTO"
                    if (cellText.StartsWith(lbl) && cellText.Length > lbl.Length) score = 3;
                    else if (cellText == lbl) score = 2;
                    else if (cellText.Contains(lbl)) score = 1;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = c;
                    }
                }
            }

            return best;
        }

        public static IXLCell? FindTotalBelowIgv(IXLWorksheet ws)
        {
            // 1) Encuentra IGV (acepta "IGV", "IGV :", "IGV (18%)", etc.)
            var igvCell = FindLabelCell(ws, "IGV");
            if (igvCell == null) return null;

            // 2) TOTAL está 1 fila abajo, misma columna del label
            var r = igvCell.Address.RowNumber + 1;
            var c = igvCell.Address.ColumnNumber;

            var totalLabelCell = ws.Cell(r, c);

            // 3) Validación: debe decir TOTAL (por si el formato cambia)
            var raw = totalLabelCell.GetFormattedString();
            if (string.IsNullOrWhiteSpace(raw)) raw = totalLabelCell.GetString();

            var txt = Normalize(raw);

            if (!txt.StartsWith("TOTAL")) return null;

            return totalLabelCell;
        }

        public static decimal? ReadNumberFromSameRow(IXLCell labelCell)
        {
            var row = labelCell.Address.RowNumber;
            var startCol = labelCell.Address.ColumnNumber + 1;
            var ws = labelCell.Worksheet;

            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? startCol;

            for (int col = startCol; col <= lastCol; col++)
            {
                var cell = ws.Cell(row, col);

                var val = SafeDecimal(cell);
                if (val.HasValue)
                    return val.Value;
            }

            return null;
        }

        public static decimal? ReadMoneyFromSameRowSkipPercent(IXLCell labelCell)
        {
            var row = labelCell.Address.RowNumber;
            var startCol = labelCell.Address.ColumnNumber + 1;
            var ws = labelCell.Worksheet;

            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? startCol;

            decimal? best = null;

            for (int col = startCol; col <= lastCol; col++)
            {
                var cell = ws.Cell(row, col);
                var fmt = (cell.GetFormattedString() ?? "").Trim();

                // ❌ ignora porcentajes
                if (fmt.Contains("%")) continue;

                // 1) número real
                var val = SafeDecimal(cell);

                // 2) si SafeDecimal no lo agarra (texto "$29,709.53"), parsea del string
                if (!val.HasValue)
                    val = TryParseMoneyLike(fmt);

                if (!val.HasValue) continue;

                // quédate con el más grande (para que gane el monto)
                if (!best.HasValue || Math.Abs(val.Value) > Math.Abs(best.Value))
                    best = val.Value;

                // si ya parece moneda, corta
                if (fmt.Contains("$")) return best.Value;
            }

            return best;
        }

       
        private static decimal? TryParseMoneyLike(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            var t = s.Trim();

            // (1,234.56) negativo
            bool neg = false;
            if (t.StartsWith("(") && t.EndsWith(")"))
            {
                neg = true;
                t = t.Substring(1, t.Length - 2);
            }

            t = t.Replace("$", "").Replace("S/", "").Replace(",", "").Trim();
            if (t == "-" || t == "" || t == "0" || t == "0.00") return null;

            if (decimal.TryParse(t, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
                return neg ? -d : d;

            if (decimal.TryParse(t, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.CurrentCulture, out d))
                return neg ? -d : d;

            return null;
        }
        private static decimal? TryParseMoeneyLike(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // Limpieza básica
            var t = s.Trim();

            // Caso paréntesis como negativo: (1,234.56)
            bool neg = false;
            if (t.StartsWith("(") && t.EndsWith(")"))
            {
                neg = true;
                t = t.Substring(1, t.Length - 2);
            }

            // Quita símbolos y separadores
            t = t.Replace("$", "").Replace("S/", "").Replace(",", "").Trim();

            // Si queda algo tipo "-" o vacío, bye
            if (t == "-" || t == "" || t == "0" || t == "0.00") return null;

            // Parse invariante (punto decimal)
            if (decimal.TryParse(t, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
            {
                return neg ? -d : d;
            }

            // Último intento con cultura actual (por si coma decimal)
            if (decimal.TryParse(t, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.CurrentCulture, out d))
            {
                return neg ? -d : d;
            }

            return null;
        }
        public static string? ReadTextFromSameRow(IXLCell labelCell, int offset = 1)
        {
            if (labelCell == null) return null;

            var cell = labelCell.Worksheet
                .Cell(labelCell.Address.RowNumber, labelCell.Address.ColumnNumber + offset);

            var value = cell.GetString()?.Trim();

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static int? ReadIntFromTextFromSameRow(IXLCell labelCell, int offset = 1)
        {
            if (labelCell == null) return null;

            var cell = labelCell.Worksheet
                .Cell(labelCell.Address.RowNumber, labelCell.Address.ColumnNumber + offset);

            // ⚠️ GetFormattedString lee el resultado de la fórmula
            var text = cell.GetFormattedString()?.Trim();

            if (string.IsNullOrWhiteSpace(text)) return null;

            // Busca el primer número (ej: "15 DÍAS." → 15)
            var match = Regex.Match(text, @"\d+");

            if (!match.Success) return null;

            return int.Parse(match.Value);
        }


        public static int? SafeInt(IXLCell? cell)
        {
            if (cell == null) return null;

            // Caso 1: valor numérico real
            if (cell.DataType == XLDataType.Number)
            {
                var n = cell.GetDouble();
                if (double.IsNaN(n)) return null;
                return (int)Math.Round(n);
            }

            // Caso 2: texto
            var s = cell.GetString()?.Trim();

            if (string.IsNullOrWhiteSpace(s)) return null;
            if (s == "-" || s == "--") return null;

            s = s.Replace(",", "").Replace(" ", "");

            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                return i;

            // fallback: intenta como decimal (Excel a veces manda 5.0)
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return (int)Math.Round(d);

            return null;
        }

        public static IXLCell? FindTitleCell(IXLWorksheet ws, string title)
        {
            var t = title.Trim().ToUpperInvariant();
            return ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c =>
                    c.DataType == XLDataType.Text &&
                    c.GetString().Trim().ToUpperInvariant() == t
                );
        }

        public static IXLCell? FindLabelInRows(IXLWorksheet ws, int fromRow, int toRow, params string[] labels)
        {
            var set = labels.Select(x => x.Trim().ToUpperInvariant()).ToHashSet();

            for (int row = fromRow; row <= toRow; row++)
            {
                foreach (var cell in ws.Row(row).CellsUsed(XLCellsUsedOptions.All))
                {
                    if (cell.DataType != XLDataType.Text) continue;
                    var s = cell.GetString().Trim().ToUpperInvariant();
                    if (set.Contains(s)) return cell;
                }
            }

            return null;
        }

        public static IXLCell? FindHeaderCell(IXLWorksheet ws, string headerText)
        {
            return ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim().Equals(headerText, StringComparison.OrdinalIgnoreCase));
        }

        public static Dictionary<string, int> BuildHeaderMap(IXLWorksheet ws, int headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // mapeamos todos los textos del row header
            foreach (var c in ws.Row(headerRow).CellsUsed(XLCellsUsedOptions.All))
            {
                var text = c.GetString().Trim();
                if (string.IsNullOrWhiteSpace(text)) continue;

                // normaliza algunos
                text = text.Replace("DESCRIPCION", "DESCRIPCIÓN", StringComparison.OrdinalIgnoreCase);

                if (!map.ContainsKey(text))
                    map[text] = c.Address.ColumnNumber;
            }

            return map;
        }
       
        public static bool IsRowRed(IXLWorksheet ws, int row, int col)
        {
            try
            {
                var cell = ws.Cell(row, col);

                // Si la celda está vacía o no tiene color de fuente, retornar false
                if (cell.IsEmpty() || cell.Style.Font.FontColor == null || !cell.Style.Font.FontColor.HasValue)
                {
                    return false;
                }

                var color = cell.Style.Font.FontColor;

                // Verificar si es rojo (RGB: 255,0,0 o similar)
                return color.Color.ToArgb() == System.Drawing.Color.Red.ToArgb() ||
                       (color.Color.R > 200 && color.Color.G < 100 && color.Color.B < 100);
            }
            catch
            {
                return false;
            }
        }

        public static string? NormalizeDisplayNo(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Trim();

            // "1." se queda "1."
            if (Regex.IsMatch(raw, @"^\d+\.$")) return raw;

            // "1.1" se queda "1.1"
            if (Regex.IsMatch(raw, @"^\d+\.\d+$")) return raw;

            // Si viene "1.2 " o "1.2\t" igual
            var m = Regex.Match(raw, @"^\d+\.\d+$");
            if (m.Success) return m.Value;

            return raw;
        }
        public static void TrySetDecimal(
           Dictionary<string, int> colMap,
           string key,
           IXLWorksheet ws,
           int row,
           Action<decimal?> setter)
        {
            if (!colMap.TryGetValue(key, out var col) || col <= 0) return;
            setter(SafeDecimal(ws.Cell(row, col)));
        }

        public static void TrySetPercent(
            Dictionary<string, int> colMap,
            string key,
            IXLWorksheet ws,
            int row,
            Action<decimal?> setter)
        {
            if (!colMap.TryGetValue(key, out var col) || col <= 0) return;
            setter(SafePercent(ws.Cell(row, col)));
        }

        public static void TrySetText(
            Dictionary<string, int> colMap,
            string key,
            IXLWorksheet ws,
            int row,
            Action<string?> setter)
        {
            if (!colMap.TryGetValue(key, out var col) || col <= 0) return;
            var v = ws.Cell(row, col).GetString().Trim();
            setter(string.IsNullOrWhiteSpace(v) ? null : v);
        }

        public static void TrySetInt(
            Dictionary<string, int> colMap,
            string key,
            IXLWorksheet ws,
            int row,
            Action<int?> setter)
        {
            if (!colMap.TryGetValue(key, out var col) || col <= 0) return;
            var cell = ws.Cell(row, col);
            if (cell.TryGetValue<int>(out var v)) setter(v);
            else
            {
                var s = cell.GetString().Trim();
                setter(int.TryParse(s, out var v2) ? v2 : null);
            }
        }


    }

}
