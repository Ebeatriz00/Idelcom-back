using Application.Contracts;
using Application.DTOs.Quotation;
using ClosedXML.Excel;
using System.Globalization;

namespace Application.Services.Excel
{
    public class QuotationExcelValidator : IQuotationExcelValidator
    {
        private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "MATERIAL", "CONSUMIBLE", "EQUIPO", "ACCESORIO", "COMPONENTE",
            "SERVICIOS", "S.EXPERTO", "GENERALES", "SOFTWARE"
        };

        private static readonly string[] RequiredColumns =
        {
            "ITEM", "DESCRIPCIÓN", "TIPO", "MED.", "CANT.", "P.U", "P.V.T."
        };

        public QuotationExcelValidationResponseDto Validate(IXLWorkbook workbook)
        {
            var result = new QuotationExcelValidationResponseDto();

            // R1 — Hoja obligatoria
            IXLWorksheet? ws = null;
            try { ws = workbook.Worksheets.FirstOrDefault(s => s.Name == "COTIZACIÓN"); }
            catch { }

            if (ws == null)
            {
                result.Errors.Add(Error("COTIZACIÓN", null, null, "No se encontró la hoja 'COTIZACIÓN'."));
                result.IsValid = false;
                return result;
            }

            // R2 — Fila de encabezado: buscar "ITEM" en columna A dentro de las primeras 30 filas
            int headerRow = -1;
            for (int r = 1; r <= 30; r++)
            {
                var val = CellText(ws.Cell(r, 1));
                if (val.Equals("ITEM", StringComparison.OrdinalIgnoreCase))
                {
                    headerRow = r;
                    break;
                }
            }

            if (headerRow == -1)
            {
                result.Errors.Add(Error("COTIZACIÓN", null, null,
                    "No se encontró la fila de encabezado con 'ITEM' en la columna A (primeras 30 filas)."));
                result.IsValid = false;
                return result;
            }

            // Construir mapa columna-nombre → índice
            var colMap = BuildColumnMap(ws, headerRow);

            foreach (var col in RequiredColumns)
            {
                if (!colMap.ContainsKey(col))
                    result.Errors.Add(Error("COTIZACIÓN", headerRow, col,
                        $"Falta la columna obligatoria '{col}' en el encabezado."));
            }

            // R3 — Detectar variante leyendo la columna 10 del encabezado
            var col10 = CellText(ws.Cell(headerRow, 10));
            if (col10.Equals("DIA", StringComparison.OrdinalIgnoreCase))
            {
                result.DetectedVariant = "B";
            }
            else if (col10.Equals("P.T.", StringComparison.OrdinalIgnoreCase))
            {
                result.DetectedVariant = "A";
            }
            else
            {
                result.Warnings.Add(
                    $"La columna 10 del encabezado tiene el valor '{col10}' " +
                    "(se esperaba 'P.T.' o 'DIA'). Se asume Variante A.");
                result.DetectedVariant = "A";
            }

            // Si faltan columnas obligatorias no podemos validar las filas de detalle
            if (result.Errors.Count > 0)
            {
                result.IsValid = false;
                return result;
            }

            int colTipo = colMap["TIPO"];
            int colDesc = colMap["DESCRIPCIÓN"];
            int colQty  = colMap["CANT."];
            int colPU   = colMap["P.U"];
            int colPVT  = colMap["P.V.T."];

            int colDia = 0;
            if (result.DetectedVariant == "B")
                colMap.TryGetValue("DIA", out colDia);

            // R4 + R5 + R6 — Filas de detalle
            int processedRows = 0;
            bool foundSubTotal = false;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow + 500;

            for (int r = headerRow + 1; r <= lastRow; r++)
            {
                var descVal = CellText(ws.Cell(r, colDesc));
                var tipoVal = CellText(ws.Cell(r, colTipo));

                // Corte: fila SUB TOTAL
                if (descVal.StartsWith("SUB TOTAL", StringComparison.OrdinalIgnoreCase) ||
                    tipoVal.StartsWith("SUB TOTAL", StringComparison.OrdinalIgnoreCase))
                {
                    foundSubTotal = true;

                    // R6 — SUB TOTAL con P.V.T. en 0 o vacío
                    var pvtTotal = CellDecimal(ws.Cell(r, colPVT));
                    if (pvtTotal == null || pvtTotal == 0m)
                        result.Warnings.Add(
                            $"La fila SUB TOTAL (fila {r}) tiene P.V.T. en 0 o vacío.");
                    break;
                }

                // Solo procesar filas donde TIPO no esté vacío
                if (string.IsNullOrWhiteSpace(tipoVal))
                    continue;

                processedRows++;

                // DESCRIPCIÓN no vacía
                if (string.IsNullOrWhiteSpace(descVal))
                    result.Errors.Add(Error("COTIZACIÓN", r, "DESCRIPCIÓN",
                        $"Fila {r}: DESCRIPCIÓN está vacía."));

                // CANT. numérico > 0
                var qty = CellDecimal(ws.Cell(r, colQty));
                if (qty == null || qty <= 0m)
                    result.Errors.Add(Error("COTIZACIÓN", r, "CANT.",
                        $"Fila {r}: CANT. debe ser numérico y mayor a 0."));

                // P.U numérico no negativo
                var pu = CellDecimal(ws.Cell(r, colPU));
                if (pu == null || pu < 0m)
                    result.Errors.Add(Error("COTIZACIÓN", r, "P.U",
                        $"Fila {r}: P.U debe ser numérico y no negativo."));

                // P.V.T. numérico
                var pvt = CellDecimal(ws.Cell(r, colPVT));
                if (pvt == null)
                    result.Errors.Add(Error("COTIZACIÓN", r, "P.V.T.",
                        $"Fila {r}: P.V.T. debe ser numérico."));

                // TIPO dentro de los valores permitidos
                if (!AllowedTypes.Contains(tipoVal))
                    result.Errors.Add(Error("COTIZACIÓN", r, "TIPO",
                        $"Fila {r}: TIPO '{tipoVal}' no está entre los valores permitidos " +
                        "(MATERIAL, CONSUMIBLE, EQUIPO, ACCESORIO, COMPONENTE, SERVICIOS, S.EXPERTO, GENERALES, SOFTWARE)."));

                // Variante B: DIA numérico > 0
                if (result.DetectedVariant == "B")
                {
                    if (colDia > 0)
                    {
                        var dia = CellDecimal(ws.Cell(r, colDia));
                        if (dia == null || dia <= 0m)
                            result.Errors.Add(Error("COTIZACIÓN", r, "DIA",
                                $"Fila {r}: DIA debe ser numérico y mayor a 0 (Variante B)."));
                    }
                    else
                    {
                        // columna DIA no encontrada en Variante B → warning una sola vez
                        if (processedRows == 1)
                            result.Warnings.Add(
                                "Variante B detectada pero no se encontró la columna 'DIA' en el encabezado.");
                    }
                }
            }

            // R5 — Al menos un ítem
            if (processedRows == 0)
                result.Errors.Add(Error("COTIZACIÓN", null, null,
                    "No se encontró ninguna fila de detalle con TIPO definido."));

            // R6 — SUB TOTAL ausente
            if (!foundSubTotal)
                result.Warnings.Add("No se encontró la fila 'SUB TOTAL'. La cotización puede estar incompleta.");

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        // ─── helpers ────────────────────────────────────────────────────────────

        private static QuotationExcelValidationError Error(
            string sheet, int? row, string? column, string message) =>
            new() { Sheet = sheet, Row = row, Column = column, Message = message };

        private static Dictionary<string, int> BuildColumnMap(IXLWorksheet ws, int headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 50;

            for (int c = 1; c <= lastCol; c++)
            {
                var text = CellText(ws.Cell(headerRow, c));
                if (!string.IsNullOrWhiteSpace(text) && !map.ContainsKey(text))
                    map[text] = c;
            }

            return map;
        }

        private static string CellText(IXLCell cell)
        {
            var c = cell.IsMerged() ? cell.MergedRange().FirstCell() : cell;
            return (c.GetString() ?? "").Trim();
        }

        private static decimal? CellDecimal(IXLCell cell)
        {
            var c = cell.IsMerged() ? cell.MergedRange().FirstCell() : cell;

            if (c.TryGetValue<double>(out var d))
                return (decimal)d;

            var s = (c.GetFormattedString() ?? "").Trim().Replace(",", "");
            if (!string.IsNullOrWhiteSpace(s) &&
                decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
                return dec;

            return null;
        }
    }
}
