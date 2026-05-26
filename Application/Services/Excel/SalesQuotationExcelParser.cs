using Application.DTOs.Quotation;
using Application.Exceptions;
using Application.Services.InterfacesServices;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ExcelParsingErrorExecption;


namespace Application.Services.Excel
{
    public class SalesQuotationExcelParser : ISalesQuotationExcelParserServices
    {
        public SalesQuotationCreateDto Parse(Stream excelStream)
        {
            if (excelStream.CanSeek)
                excelStream.Position = 0;

            try
            {

                using var sanitized = ExcelSanitizer.SanitizeBrokenRefs(excelStream);
                using var workbook = new XLWorkbook(sanitized);
                return ParseWorkbook(workbook);

            }
            catch (AppValidationException) 
            {
                throw;
            }
            catch (Exception ex) when (IsBrokenRef(ex))
            {
                if (excelStream.CanSeek)
                    excelStream.Position = 0;

                try
                {
                    using var fixedStream = SharedKernel.Helpers.Helpers.RemoveBrokenDefinedNames(excelStream);
                    using var workbook = new XLWorkbook(fixedStream);
                    return ParseWorkbook(workbook);
                }
                catch (AppValidationException)
                {
                    throw;
                }
                catch (Exception ex2)
                {
                    throw MapToExcelParsingException(ex2);
                }
            }
            catch (Exception ex)
            {
                throw MapToExcelParsingException(ex);
            }
        }
        
        private SalesQuotationCreateDto ParseWorkbook(XLWorkbook workbook)
        {
            var ws = workbook.Worksheet("COTIZACIÓN");
            var wx = workbook.Worksheet("Datos Comerciales");

            ValidateHeader(ws);
            ValidateHeaderOrder(ws);
            ValidateVvcAndResumenSection(ws);
            if (!IsDayQuotationVariant(ws))
                ValidateSeedCapitalSection(ws);

            var dto = new SalesQuotationCreateDto();

            ParseHeader(ws, wx, dto);
            ParseTotals(ws, dto);
            dto.Margins = ParseMargins(ws);
            dto.ServChecks = ParseServiceCheck(ws);

            // =========================
            // 5) DETALLE (tabla azul)
            // =========================
            var detailParse = ParseDetailLines(ws);

            // =========================
            // 6) PREVENTA (tabla gris)
            // =========================
            ApplyPresalesTable(
                ws,
                detailParse.AllLines.Where(x => x.LineType == QuotationLineType.Item).ToList(),
                detailParse.LineNoToExcelRow
            );

            var allItems = detailParse.AllLines.Where(x => x.LineType == QuotationLineType.Item).ToList();
            dto.Lines = detailParse.AllLines;

            // 7) PAGOS (hasta 12)
            dto.LinePlans = ParseLinePlans(ws, allItems, detailParse.LineNoToExcelRow);

            // 8) EGRESOS (hasta 12)
            dto.Egresses = ParseEgresses(ws, allItems, detailParse.LineNoToExcelRow);

            return dto;
        }

        private static void ParseHeader(IXLWorksheet ws, IXLWorksheet wx, SalesQuotationCreateDto dto)
        {
            // =========================
            // CABECERA
            // =========================
            var cotCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .Select(MasterCell)
                .FirstOrDefault(c => (c.GetString() ?? "").Contains("COT", StringComparison.OrdinalIgnoreCase));

            if (cotCell != null)
            {
                var raw = (cotCell.GetString() ?? "");
                dto.OpporNumber = Regex.Match(raw, @"ID\d+", RegexOptions.IgnoreCase).Value;
            }

            var clientsCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "CLIENTE");
            if (clientsCell != null)
                dto.ClientsName = ReadTextToRight(ws, clientsCell);

            var opporNameCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "PROYECTO");
            if (opporNameCell != null)
                dto.OpporName = ReadTextToRight(ws, opporNameCell);

            var startDateCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "FECHA");
            if (startDateCell != null)
                dto.StartDate = ReadDateToRight(ws, startDateCell) ?? default;

            var finishDateCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "VALIDO HASTA");
            if (finishDateCell != null)
                dto.FinishDate = ReadDateToRight(ws, finishDateCell) ?? default;

            var currencyLabel = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "TIPO DE MONEDA");
            if (currencyLabel != null)
                dto.CurrencyName = ReadTextToRight(ws, currencyLabel);
            else
                dto.CurrencyName = ws.Cell("L14").GetString();

            dto.ExchangeRate = SharedKernel.Helpers.Helpers.SafeDecimal(wx.Cell("H2"));

            var paymentCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "Forma de pago");
            if (paymentCell != null)
                dto.PaymentConditionName = ReadTextToRight(ws, paymentCell);

            var validezCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "Validez de la oferta");
            if (validezCell != null)
                dto.OfferValidity = Helpers.ReadIntFromTextFromSameRow(validezCell);
        }

        private static void ParseTotals(IXLWorksheet ws, SalesQuotationCreateDto dto)
        {
            // =========================
            // TOTALES
            // =========================
            var subTotalCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "SUB TOTAL");
            if (subTotalCell != null)
                dto.SubTotal = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(subTotalCell);

            var discountCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "DESCUENTO");
            if (discountCell != null)
                dto.DiscountAmount = SharedKernel.Helpers.Helpers.ReadMoneyFromSameRowSkipPercent(discountCell);

            var igvCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "IGV");
            if (igvCell != null)
                dto.TaxAmount = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(igvCell);

            var totalCell = SharedKernel.Helpers.Helpers.FindTotalBelowIgv(ws);
            if (totalCell != null)
                dto.Total = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(totalCell);

            var vvcCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "V.V.C.");
            if (vvcCell != null)
                dto.VvcTotal = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(vvcCell);

            var salesTotalCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "V. TOTAL");
            if (salesTotalCell != null)
                dto.SalesTotal = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(salesTotalCell);

            var costTotalCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "C. TOTAL");
            if (costTotalCell != null)
                dto.CostTotal = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(costTotalCell);

            var utilyTotalCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "U. TOTAL");
            if (utilyTotalCell != null)
                dto.UtilityTotal = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(utilyTotalCell);

            var marginPorcentCell = SharedKernel.Helpers.Helpers.FindLabelCell(ws, "M. TOTAL");
            if (marginPorcentCell != null)
                dto.MarginPercent = SharedKernel.Helpers.Helpers.SafePercent(marginPorcentCell);
        }

        private static List<SalesQuotationMarginDto> ParseMargins(IXLWorksheet ws)
        {
            // =========================
            // 3) MARGENES + DESCUENTO
            // =========================
            var margins = new List<SalesQuotationMarginDto>();

            var titleMargins = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim().Equals("MARGENES", StringComparison.OrdinalIgnoreCase));

            if (titleMargins != null)
            {
                var range = titleMargins.IsMerged()
                    ? titleMargins.MergedRange()
                    : ws.Range(titleMargins.Address, titleMargins.Address);

                int startRow = range.LastRow().RowNumber() + 1;
                int colName = range.FirstColumn().ColumnNumber();
                int colRate = range.LastColumn().ColumnNumber();

                int emptyStreak = 0;
                int lastRow = ws.LastRowUsed()?.RowNumber() ?? startRow;

                for (int row = startRow; row <= lastRow; row++)
                {
                    var name = (MasterCell(ws.Cell(row, colName)).GetString() ?? "").Trim();
                    var rateCell = MasterCell(ws.Cell(row, colRate));

                    var hasName = !string.IsNullOrWhiteSpace(name);
                    var hasRate = rateCell.TryGetValue<double>(out _)
                                  || !string.IsNullOrWhiteSpace(rateCell.GetFormattedString());

                    if (!hasName && !hasRate)
                    {
                        emptyStreak++;
                        if (emptyStreak >= 3) break;
                        continue;
                    }

                    emptyStreak = 0;
                    if (!hasName) continue;

                    margins.Add(new SalesQuotationMarginDto
                    {
                        MarginTypeName = name,
                        MarginRate = SharedKernel.Helpers.Helpers.SafePercent(rateCell)
                    });
                }
            }

            return margins;
        }

        private static List<SalesQuotationServCheckDto> ParseServiceCheck(IXLWorksheet ws)
        {
            // =========================
            // 4) COMPROBACIÓN SERVICIOS
            // =========================
            SalesQuotationServCheckDto servCheck = new();

            var titleCell = SharedKernel.Helpers.Helpers.FindTitleCell(ws, "COMPROBACIÓN DE SERVICIOS");
            if (titleCell != null)
            {
                int startRow = titleCell.IsMerged()
                    ? titleCell.MergedRange().LastRow().RowNumber() + 1
                    : titleCell.Address.RowNumber + 1;

                int endRow = Math.Min(startRow + 15, ws.LastRowUsed()?.RowNumber() ?? startRow + 15);

                var coti = SharedKernel.Helpers.Helpers.FindLabelInRows(ws, startRow, endRow, "COTIZACIÓN", "COTIZACION");
                if (coti != null)
                    servCheck.QuotationAmount = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(coti);

                var cron = SharedKernel.Helpers.Helpers.FindLabelInRows(ws, startRow, endRow, "CRONOGRAMA");
                if (cron != null)
                    servCheck.ScheduleAmount = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(cron);

                var diff = SharedKernel.Helpers.Helpers.FindLabelInRows(ws, startRow, endRow, "DIFERENCIA");
                if (diff != null)
                    servCheck.DifferenceAmount = SharedKernel.Helpers.Helpers.ReadNumberFromSameRow(diff);
            }

            var servChecks = new List<SalesQuotationServCheckDto>();
            if (servCheck.QuotationAmount.HasValue || servCheck.ScheduleAmount.HasValue || servCheck.DifferenceAmount.HasValue)
                servChecks.Add(servCheck);

            return servChecks;
        }

        // ==========================================================
        // Helpers: Ocultos / merges / lectura a la derecha
        // ==========================================================

        private static IXLCell MasterCell(IXLCell c)
            => c.IsMerged() ? c.MergedRange().FirstCell() : c;

        private static string ReadTextToRight(IXLWorksheet ws, IXLCell labelCell, int maxLookahead = 30)
        {
            labelCell = MasterCell(labelCell);

            int row = labelCell.Address.RowNumber;
            int col = labelCell.Address.ColumnNumber;

            for (int i = 1; i <= maxLookahead; i++)
            {
                var c = MasterCell(ws.Cell(row, col + i));

                var txt = (c.GetString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(txt))
                    txt = (c.GetFormattedString() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(txt))
                    return txt;
            }

            return "";
        }

        private static DateTime? ReadDateToRight(IXLWorksheet ws, IXLCell labelCell, int maxLookahead = 30)
        {
            labelCell = MasterCell(labelCell);

            int row = labelCell.Address.RowNumber;
            int col = labelCell.Address.ColumnNumber;

            for (int i = 1; i <= maxLookahead; i++)
            {
                var c = MasterCell(ws.Cell(row, col + i));

                if (c.DataType == XLDataType.DateTime)
                    return c.GetDateTime();

                if (c.DataType == XLDataType.Number)
                {
                    try { return DateTime.FromOADate(c.GetDouble()); } catch { }
                }

                var raw = (c.GetFormattedString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(raw)) raw = (c.GetString() ?? "").Trim();

                if (DateTime.TryParse(raw, out var dt))
                    return dt;
            }

            return null;
        }
        private sealed class DetailParseResult
        {
            public List<SalesQuotationLinDto> AllLines { get; } = new();
            public List<SalesQuotationLinDto> PresalesLinesInOrder { get; } = new();

            public bool LastInsertedWasGroup { get; private set; }
            public string? LastGroupInternalKey { get; private set; }

            public Dictionary<int, int> LineNoToExcelRow { get; } = new();

            public void AddLine(
                SalesQuotationLinDto line,
                int excelRow,
                string? parentDisplayNo = null,
                string? parentInternalKey = null)
            {
                AllLines.Add(line);

                if (line.LineNo.HasValue)
                    LineNoToExcelRow[line.LineNo.Value] = excelRow;

                LastInsertedWasGroup = string.Equals(line.LineType, QuotationLineType.Group, StringComparison.OrdinalIgnoreCase);
                LastGroupInternalKey = LastInsertedWasGroup ? MakeKey(line) : LastGroupInternalKey;
            }

            private static string MakeKey(SalesQuotationLinDto line)
                => $"{line.LineNo}|{line.DisplayNo ?? ""}|{(line.LineType ?? "")}";
        }

        private DetailParseResult ParseDetailLines(IXLWorksheet ws)
        {
            var result = new DetailParseResult();

            var headerCell = SharedKernel.Helpers.Helpers.FindHeaderCell(ws, "ITEM");
            if (headerCell == null) return result;

            int headerRow = headerCell.Address.RowNumber;
            var colMap = SharedKernel.Helpers.Helpers.BuildHeaderMap(ws, headerRow);

            if (!colMap.TryGetValue("ITEM", out var colItem) ||
                !colMap.TryGetValue("DESCRIPCIÓN", out var colDesc))
                return result;

            colMap.TryGetValue("TIPO", out var colTipo);
            colMap.TryGetValue("MED.", out var colUom);
            colMap.TryGetValue("MARCA", out var colBrand);
            colMap.TryGetValue("MODELO", out var colModel);
            colMap.TryGetValue("CANT.", out var colQty);
            colMap.TryGetValue("P.U", out var colUnit);
            colMap.TryGetValue("P.T.", out var colPT);
            colMap.TryGetValue("DIA", out var colDia);   // Variante B
            colMap.TryGetValue("P.V.T.", out var colPVT);

            int row = headerRow + 1;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? row + 300;

            int lineNo = 0;
            int emptyCount = 0;

            // Camino jerárquico por nivel real (1..N)
            var currentByLevel = new Dictionary<int, string>();

            // Último leaf numerado real (para colgar filas sin ITEM)
            string? lastLeafDisplayNo = null;

            // Grupo (tu lógica original)
            string? currentGroup = null;

            // Secuencia de items por “padre lógico”
            int itemSeq = 0;
            string? lastParentKey = null;

            for (; row <= lastRow; row++)
            {
                var desc = ws.Cell(row, colDesc).GetString()?.Trim() ?? "";
                var itemTxt = ws.Cell(row, colItem).GetString()?.Trim() ?? "";

                // Corte
                if (desc.StartsWith("SUB TOTAL", StringComparison.OrdinalIgnoreCase) ||
                    itemTxt.StartsWith("SUB TOTAL", StringComparison.OrdinalIgnoreCase))
                    break;

                // Fila vacía
                if (string.IsNullOrWhiteSpace(desc) && string.IsNullOrWhiteSpace(itemTxt))
                {
                    if (++emptyCount >= 5) break;
                    continue;
                }
                emptyCount = 0;

                // 🔴 PREVENTA: SOLO POR COLOR
                bool isRed =
                    SharedKernel.Helpers.Helpers.IsRowRed(ws, row, colDesc) ||
                    SharedKernel.Helpers.Helpers.IsRowRed(ws, row, colItem);

                var line = new SalesQuotationLinDto
                {
                    LineNo = ++lineNo,
                    Description = desc,
                    IsBold = ws.Cell(row, colDesc).Style.Font.Bold
                };

                if (colTipo > 0) line.ProductsTypeName = ws.Cell(row, colTipo).GetString()?.Trim();
                if (colUom > 0) line.UomName = ws.Cell(row, colUom).GetString()?.Trim();
                if (colBrand > 0) line.Brands = ws.Cell(row, colBrand).GetString()?.Trim();
                if (colModel > 0) line.Model = ws.Cell(row, colModel).GetString()?.Trim();
                if (colQty > 0) line.Qty = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(row, colQty));
                if (colUnit > 0) line.UnitPrice = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(row, colUnit));

                if (colPT > 0)
                {
                    line.TotalPrice = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(row, colPT));
                }
                else if (colDia > 0)
                {
                    var dia = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(row, colDia));
                    if (dia.HasValue)
                        line.TotalPrice = dia;
                }

                if (colPVT > 0) line.LineAmount = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(row, colPVT));

                
                bool hasItemValues =
                    line.Qty != null ||
                    line.UnitPrice != null ||
                    line.TotalPrice != null;


                // ===== ITEM FLEXIBLE (C.40.04 / 07.02.07 / etc.) =====
                var hasItemNo = TryParseFlexibleItemKey(itemTxt, out var displayNoNorm, out var itemLevel, out var explicitParent);

                if (hasItemNo)
                {
                    line.DisplayNo = displayNoNorm;

                    // Nodo jerárquico (rollup): viene con ITEM pero sin montos/cantidades
                    bool isNode =
                             !hasItemValues 
                             || NextItemIsDeeper(ws, row, lastRow, colItem, itemLevel);

                    if (isNode)
                    {
                        currentByLevel[itemLevel] = displayNoNorm!;
                        ClearDeeperLevels(currentByLevel, itemLevel);

                        // cambio de bloque
                        currentGroup = null;
                        lastLeafDisplayNo = null;

                        ResetSeqIfParentChanged(displayNoNorm, ref itemSeq, ref lastParentKey);

                        line.LevelNo = ClampLevel(itemLevel); // si tu sistema solo usa 1..3
                        line.LineType = itemLevel == 1 ? QuotationLineType.Parent : QuotationLineType.Child;
                        line.IsRollUp = true;
                        line.IsPresales = false;

                        // parent del nodo: lo que diga explicitParent o el nivel anterior
                        var parentDisplayNo =
                            explicitParent
                            ?? GetParentDisplayNo(currentByLevel, itemLevel)
                            ?? GetNearestParent(currentByLevel);

                        result.AddLine(line, excelRow: row, parentDisplayNo: parentDisplayNo);
                        continue;
                    }
                    else
                    {
                        // Leaf real numerado (ítem con montos)
                        var parentDisplayNo =
                            explicitParent
                            ?? GetParentDisplayNo(currentByLevel, itemLevel)
                            ?? GetNearestParent(currentByLevel);

                        lastLeafDisplayNo = displayNoNorm; // 👈 clave para filas sin ITEM debajo
                        currentGroup = null;

                        ResetSeqIfParentChanged(parentDisplayNo ?? lastLeafDisplayNo, ref itemSeq, ref lastParentKey);

                        line.LevelNo = ClampLevel(itemLevel);
                        line.LineType = QuotationLineType.Item;
                        line.IsRollUp = false;
                        line.IsPresales = isRed;

                        line.ItemId = ++itemSeq;

                        result.AddLine(line, excelRow: row, parentDisplayNo: parentDisplayNo);

                        if (line.IsPresales)
                            result.PresalesLinesInOrder.Add(line);

                        continue;
                    }
                }
                // ===== GROUP (igual que antes, pero cuelga de un parent válido) =====
                bool looksLikeGroup =
                    string.IsNullOrWhiteSpace(itemTxt) &&
                    line.IsBold &&
                    (line.Qty == null && line.UnitPrice == null);

                if (looksLikeGroup)
                {
                    currentGroup = desc;

                    var parentForGroup =
                        GetNearestParent(currentByLevel)
                        ?? lastLeafDisplayNo;

                    ResetSeqIfParentChanged(parentForGroup, ref itemSeq, ref lastParentKey);

                    line.LevelNo = 3;
                    line.LineType = QuotationLineType.Group;
                    line.IsRollUp = true;
                    line.IsPresales = false;

                    line.DisplayNo = null;
                    result.AddLine(line, excelRow: row, parentDisplayNo: parentForGroup);
                    continue;
                }


                // ===== FILA SIN ITEM: cuelga del último leaf numerado o del parent más cercano =====
                {
                    var parentDisplayNo =
                        lastLeafDisplayNo
                        ?? GetNearestParent(currentByLevel);

                    ResetSeqIfParentChanged(parentDisplayNo, ref itemSeq, ref lastParentKey);

                    line.DisplayNo = null;
                    line.LevelNo = 3;
                    line.LineType = QuotationLineType.Item;
                    line.IsRollUp = false;
                    line.IsPresales = isRed;

                    line.ItemId = ++itemSeq;

                    result.AddLine(line, excelRow: row, parentDisplayNo: parentDisplayNo);

                    if (line.IsPresales)
                        result.PresalesLinesInOrder.Add(line);
                }
            }

            return result;
        }

        private static int ClampLevel(int levelReal)
        {
            // Si tu DB/UI solo maneja 1..3, lo “capas” aquí.
            // Igual la jerarquía REAL se mantiene con DisplayNo/ParentDisplayNo.
            if (levelReal <= 1) return 1;
            if (levelReal == 2) return 2;
            return 3;
        }

        private static void ResetSeqIfParentChanged(string? parentKey, ref int itemSeq, ref string? lastParentKey)
        {
            var key = parentKey ?? "__ROOT__";
            if (!string.Equals(lastParentKey, key, StringComparison.Ordinal))
            {
                itemSeq = 0;
                lastParentKey = key;
            }
        }

        /// <summary>
        /// Soporta: 07.02.07, C.40.04, C.40.04.02.04, AA-01-02, E/10/05, etc.
        /// Regla: debe tener separador (., -, /) + segmentos alfanuméricos.
        /// Normaliza todo a "." para consistencia interna.
        /// </summary>


        private static bool NextItemIsDeeper(
        IXLWorksheet ws,
        int startRow,
        int lastRow,
        int colItem,
        int currentLevel)
            {
                for (int r = startRow + 1; r <= lastRow; r++)
                {
                    var t = ws.Cell(r, colItem).GetString()?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(t)) continue;

                    if (TryParseFlexibleItemKey(t, out var _, out var nextLevel, out var _))
                    {
                        return nextLevel > currentLevel;
                    }

                    // si la siguiente cosa que aparece no es un item flexible,
                    // seguimos buscando (porque puede haber líneas sin ITEM debajo)
                }

                return false;
        }

        private static bool TryParseFlexibleItemKey(
    string? raw,
    out string? normalizedKey,
    out int level,
    out string? parentKey
)
        {
            normalizedKey = null;
            level = 0;
            parentKey = null;

            if (string.IsNullOrWhiteSpace(raw)) return false;

            var txt = raw.Trim();

            // ✅ Excel "texto": '07.05...  (y también comillas raras)
            txt = txt.TrimStart('\'', '’', '‘', '"', '“', '”');

            // Normaliza espacios invisibles (NBSP)
            txt = txt.Replace('\u00A0', ' ').Trim();

            // Caso: "1." o "1" (nivel 1)
            var onlyOne = txt.TrimEnd('.').Trim();
            if (Regex.IsMatch(onlyOne, @"^\d+$"))
            {
                normalizedKey = onlyOne;
                level = 1;
                parentKey = null;
                return true;
            }

            // Quita punto final tipo "C.40.04."
            txt = txt.TrimEnd('.');

            // Debe tener separador para ser jerárquico
            if (!Regex.IsMatch(txt, @"[.\-\/]")) return false;

            var parts = Regex.Split(txt, @"[.\-\/]+", RegexOptions.Compiled);

            var cleanParts = new List<string>();
            foreach (var p in parts)
            {
                var s = p.Trim();
                if (s.Length == 0) continue;

                // ✅ por si algún segmento viene con apóstrofe pegado (raro pero pasa)
                s = s.TrimStart('\'', '’', '‘', '"', '“', '”');

                if (!Regex.IsMatch(s, @"^[A-Za-z0-9]+$")) return false;

                cleanParts.Add(s);
            }

            if (cleanParts.Count == 0) return false;

            normalizedKey = string.Join(".", cleanParts);
            level = cleanParts.Count;

            if (level > 1)
                parentKey = string.Join(".", cleanParts.Take(level - 1));

            return true;
        }

        private static void ClearDeeperLevels(Dictionary<int, string> map, int level)
        {
            var toRemove = map.Keys.Where(k => k > level).ToList();
            foreach (var k in toRemove)
                map.Remove(k);
        }

        private static string? GetParentDisplayNo(Dictionary<int, string> map, int level)
        {
            if (level <= 1) return null;
            return map.TryGetValue(level - 1, out var p) ? p : null;
        }

        private static string? GetNearestParent(Dictionary<int, string> map)
        {
            if (map.Count == 0) return null;
            var maxLevel = map.Keys.Max();
            return map[maxLevel];
        }

        // ==========================================================
        // PREVENTA (tabla gris): acoplar 1-a-1 por orden a items rojos
        // ==========================================================

        private void ApplyPresalesTable(
            IXLWorksheet ws,
            List<SalesQuotationLinDto> presalesLines,
            Dictionary<int, int> lineNoToExcelRow
        )
        {
            if (presalesLines == null || presalesLines.Count == 0)
                return;

            var headers = ws.CellsUsed(XLCellsUsedOptions.All)
                .Where(c => c.GetString()?.Trim().Equals("P.U. Costo $", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (headers.Count == 0) return;

            var header = headers.First();
            int headerRow = header.Address.RowNumber;
            var colMap = SharedKernel.Helpers.Helpers.BuildHeaderMap(ws, headerRow);

            foreach (var line in presalesLines)
            {
                if (!line.LineNo.HasValue) continue;

                if (!lineNoToExcelRow.TryGetValue(line.LineNo.Value, out var dataRow))
                {
                    Console.WriteLine($"WARN: No ExcelRow para LineNo {line.LineNo}");
                    continue;
                }

                if (dataRow <= headerRow) continue;

                // ✅ campos costo/venta
                SharedKernel.Helpers.Helpers.TrySetDecimal(colMap, "P.U. Costo $", ws, dataRow, v => line.UnitCost = v);
                SharedKernel.Helpers.Helpers.TrySetDecimal(colMap, "Costo Total $.", ws, dataRow, v => line.LineCostTotal = v);
                SharedKernel.Helpers.Helpers.TrySetPercent(colMap, "Margen", ws, dataRow, v => line.MargenPorcentLine = v);
                SharedKernel.Helpers.Helpers.TrySetDecimal(colMap, "P.U. Venta $", ws, dataRow, v => line.UnitSalePrice = v);
                SharedKernel.Helpers.Helpers.TrySetDecimal(colMap, "Total Venta.$", ws, dataRow, v => line.LineSaleTotal = v);

                // ✅ asignaciones/catalogos (solo rojos porque presalesLines son rojos)
                SharedKernel.Helpers.Helpers.TrySetText(colMap, "PREVENTA", ws, dataRow, v => line.PresalesAssignedTo = v);

                if (string.IsNullOrWhiteSpace(line.ProductsTypeName))
                    SharedKernel.Helpers.Helpers.TrySetText(colMap, "TIPO", ws, dataRow, v => line.ProductsTypeName = v);

                SharedKernel.Helpers.Helpers.TrySetText(colMap, "SISTEMA", ws, dataRow, v => line.SystemName = v);

                if (string.IsNullOrWhiteSpace(line.SuppliersName))
                    SharedKernel.Helpers.Helpers.TrySetText(colMap, "MAYORISTA", ws, dataRow, v => line.SuppliersName = v);

                SharedKernel.Helpers.Helpers.TrySetInt(colMap, "ENTREGA (DÍAS)", ws, dataRow, v => line.DeliveryDays = v);
                SharedKernel.Helpers.Helpers.TrySetText(colMap, "CONDICIONES", ws, dataRow, v => line.PmConditionName = v);
                SharedKernel.Helpers.Helpers.TrySetInt(colMap, "CREDITO (DÍAS)", ws, dataRow, v => line.PmConditionDay = v);
                SharedKernel.Helpers.Helpers.TrySetInt(colMap, "MES PEDIDO", ws, dataRow, v => line.OrderMonthNo = v);
            }
        }

        private List<SalesQuotationLinePlanDto> ParseLinePlans(
            IXLWorksheet ws,
            List<SalesQuotationLinDto> items,
            Dictionary<int, int> lineNoToExcelRow
        )
        {
            var result = new List<SalesQuotationLinePlanDto>();
            if (items == null || items.Count == 0) return result;

            // Header fila del bloque gris
            var headerCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c =>
                    c.GetString()?.Trim().Equals("P.U. Costo $", StringComparison.OrdinalIgnoreCase) == true);

            if (headerCell == null) return result;

            int headerRow = headerCell.Address.RowNumber;
            int lastCol = Math.Min(ws.LastColumnUsed()?.ColumnNumber() ?? 250, 250);
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow;

            // 1) Detectar MES PEDIDO y pares PAGO/PEDIDO -> MONTO por ORDEN REAL
            int colMes = 0;

            var pairsCols = new List<(int colPay, int colAmt)>(12);
            int? pendingPayCol = null;

            for (int col = 1; col <= lastCol; col++)
            {
                var raw = ws.Cell(headerRow, col).GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var up = raw.ToUpperInvariant();

                if (up == "MES PEDIDO")
                {
                    colMes = col;
                    continue;
                }

                // ✅ soporta "PAGO 01" y también "PEDIDO 01"
                bool isPay = up.StartsWith("PAGO") || up.StartsWith("PEDIDO");
                bool isAmt = up.StartsWith("MONTO") || up.Contains("MONTO");

                if (isPay)
                {
                    pendingPayCol = col; // si vienen 2 seguidos, nos quedamos con el último
                    continue;
                }

                if (isAmt && pendingPayCol.HasValue)
                {
                    pairsCols.Add((pendingPayCol.Value, col));
                    pendingPayCol = null;

                    if (pairsCols.Count >= 12) break;
                }
            }

            if (colMes <= 0 || pairsCols.Count == 0) return result;

            // Helpers
            int? ReadMesPedido(int r)
            {
                var cell = ws.Cell(r, colMes);

                // numérico excel
                if (cell.TryGetValue<double>(out var d))
                {
                    var i = (int)Math.Round(d);
                    return i >= 1 ? i : (int?)null;
                }

                // texto
                var s = cell.GetFormattedString();
                if (string.IsNullOrWhiteSpace(s)) s = cell.GetString();
                s = s?.Trim();

                if (int.TryParse(s, out var v) && v >= 1) return v;

                return null;
            }

            decimal? ReadPercent(int r, int c)
            {
                var p = SharedKernel.Helpers.Helpers.SafePercent(ws.Cell(r, c));
                if (!p.HasValue) return null;
                return p.Value > 1m ? p.Value / 100m : p.Value; // 100% => 1.00
            }

            decimal? ReadMoneyCell(int r, int c)
            {
                var v = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(r, c));
                if (v.HasValue) return v;

                var s = ws.Cell(r, c).GetFormattedString()?.Trim();
                if (string.IsNullOrWhiteSpace(s) || s == "$")
                    return SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(r, c + 1));

                s = s.Replace("$", "").Replace(",", "").Trim();
                if (s == "-" || s == "$-" || s == "0" || s == "0.00") return null;

                return decimal.TryParse(s, out var p) ? p : null;
            }

            bool HasPositivePercent(decimal? p) => p.HasValue && p.Value > 0m;
            bool HasPositiveAmount(decimal? a) => a.HasValue && a.Value > 0m;

            // 🔥 Estado: último MES PEDIDO visto (para heredar cuando venga vacío)
            int? lastMesPedido = null;

            // 2) Mapear por excelRow del item
            foreach (var item in items)
            {
                if (!item.LineNo.HasValue) continue;
                if (!lineNoToExcelRow.TryGetValue(item.LineNo.Value, out var r)) continue;
                if (r <= headerRow || r > lastRow) continue;

                var mesPedido = ReadMesPedido(r);

                // ✅ si viene vacío, hereda el anterior
                if (mesPedido.HasValue)
                    lastMesPedido = mesPedido;
                else
                    mesPedido = lastMesPedido;

                if (!mesPedido.HasValue) continue; // si ni heredando hay, no registramos

                var planLines = new List<SalesQuotationLinePlanLinDto>(pairsCols.Count);

                int paymentNo = 0; // ✅ 1..n por orden en la fila

                foreach (var pair in pairsCols)
                {
                    decimal? percent = ReadPercent(r, pair.colPay);
                    decimal? amount = ReadMoneyCell(r, pair.colAmt);

                    if (!HasPositivePercent(percent) && !HasPositiveAmount(amount))
                        continue;

                    paymentNo++;

                    planLines.Add(new SalesQuotationLinePlanLinDto
                    {
                        MonthNo = mesPedido.Value,
                        PaymentNo = paymentNo,
                        SeqNo = paymentNo,
                        PaymentPercent = HasPositivePercent(percent) ? percent : null,
                        PaymentAmount = HasPositiveAmount(amount) ? amount : null
                    });
                }

                if (planLines.Count > 0)
                {
                    result.Add(new SalesQuotationLinePlanDto
                    {
                        TempLineNo = item.LineNo.Value,
                        Lines = planLines
                    });
                }
            }

            return result;
        }

        private List<SalesQuotationEgressDto> ParseEgresses(
            IXLWorksheet ws,
            List<SalesQuotationLinDto> items,
            Dictionary<int, int> lineNoToExcelRow
        )
        {
            var result = new List<SalesQuotationEgressDto>();
            if (items == null || items.Count == 0) return result;

            // ✅ Para detalle: SIN fallback a c+1 (evita que MES00 copie MES01)
            decimal? ReadMoneyCellStrict(int r, int c)
            {
                var v = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(r, c));
                if (v.HasValue) return v;

                var s = ws.Cell(r, c).GetFormattedString()?.Trim();
                if (string.IsNullOrWhiteSpace(s) || s == "$") return null; // ✅ NO fallback

                s = s.Replace("$", "").Replace(",", "").Trim();
                if (s == "-" || s == "$-" || s == "0" || s == "0.00") return null;

                return decimal.TryParse(s, out var p) ? p : null;
            }

            // ✅ Para totales/cabecera: con fallback porque a veces el $ está “raro”
            decimal? ReadMoneyCellLoose(int r, int c)
            {
                var v = SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(r, c));
                if (v.HasValue) return v;

                var s = ws.Cell(r, c).GetFormattedString()?.Trim();
                if (string.IsNullOrWhiteSpace(s) || s == "$")
                    return SharedKernel.Helpers.Helpers.SafeDecimal(ws.Cell(r, c + 1));

                s = s.Replace("$", "").Replace(",", "").Trim();
                if (s == "-" || s == "$-" || s == "0" || s == "0.00") return null;

                return decimal.TryParse(s, out var p) ? p : null;
            }

            // ---------- helpers ----------
            string Norm(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = s.Trim().ToUpperInvariant();
                return System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
            }

            bool IsMes00OrSeed(string up) =>
                up == "MES 00" || up == "MES 0" || up == "SEED CAPITAL";

            bool IsSkipDetailHeader(string up) =>
                up == "EGRESOS"
                || up == "MES 00" || up == "MES 0"
                || up == "SEED CAPITAL"
                || up == "OUTLAY" || up.StartsWith("OUTLAY");

            // 1) Buscar título EGRESOS
            var egTitle = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c =>
                    c.GetString()?.Trim().Equals("EGRESOS", StringComparison.OrdinalIgnoreCase) == true ||
                    c.GetString()?.Trim().StartsWith("EGRESOS", StringComparison.OrdinalIgnoreCase) == true);

            if (egTitle == null) return result;

            // 🔥 FIX CLAVE: el header (MES00/SEED) está arriba del EGRESOS (en tus excels reales)
            int startSearchRow = Math.Max(1, egTitle.Address.RowNumber - 5);

            int lastRow = ws.LastRowUsed()?.RowNumber() ?? egTitle.Address.RowNumber;
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 200;

            // 2) Buscar fila header donde están MES 00 / SEED CAPITAL .. MES 11
            int headerRow = -1;
            int startColMes00 = -1;

            for (int r = startSearchRow; r <= Math.Min(startSearchRow + 25, lastRow); r++)
            {
                for (int c = 1; c <= lastCol; c++)
                {
                    var up = Norm(ws.Cell(r, c).GetString());
                    if (string.IsNullOrWhiteSpace(up)) continue;

                    if (!IsMes00OrSeed(up)) continue;

                    bool ok = true;
                    for (int k = 1; k <= 11; k++)
                    {
                        var expected1 = $"MES {k:00}";
                        var expected2 = $"MES {k}";
                        var txt = Norm(ws.Cell(r, c + k).GetString());

                        if (txt != expected1 && txt != expected2)
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        headerRow = r;
                        startColMes00 = c;
                        break;
                    }
                }

                if (headerRow > 0) break;
            }

            if (headerRow < 0 || startColMes00 < 0) return result;

            // 3) Columnas fijas por posición (blindado)
            var monthCols = new Dictionary<int, int>(12);
            for (int m = 0; m <= 11; m++)
                monthCols[m] = startColMes00 + m;

            // 4) Totales por mes (fila siguiente al header)
            int totalsRow = headerRow + 1;

            for (int m = 0; m <= 11; m++)
            {
                var col = monthCols[m];
                var amt = ReadMoneyCellLoose(totalsRow, col);

                result.Add(new SalesQuotationEgressDto
                {
                    MonthNo = m,
                    Amount = amt ?? 0m
                });
            }

            // Index rápido: excelRow -> TempLineNo (lineNo)
            var excelRowToLineNo = new Dictionary<int, int>();
            foreach (var it in items)
            {
                if (!it.LineNo.HasValue) continue;
                if (lineNoToExcelRow.TryGetValue(it.LineNo.Value, out var rr))
                    excelRowToLineNo[rr] = it.LineNo.Value;
            }

            // 5) Detalle: empieza después del header
            int detailStartRow = headerRow + 2;

            for (int r = detailStartRow; r <= lastRow; r++)
            {
                var firstText = Norm(ws.Cell(r, startColMes00).GetString());
                if (IsSkipDetailHeader(firstText)) continue;

                bool hasAny = false;
                for (int m = 0; m <= 11; m++)
                {
                    var amt = ReadMoneyCellStrict(r, monthCols[m]);
                    if (amt.HasValue && amt.Value != 0m)
                    {
                        hasAny = true;
                        break;
                    }
                }

                if (!hasAny) continue;

                excelRowToLineNo.TryGetValue(r, out var tempLineNo);
                int? tempLineNoNullable = tempLineNo > 0 ? tempLineNo : (int?)null;

                for (int m = 0; m <= 11; m++)
                {
                    var amt = ReadMoneyCellStrict(r, monthCols[m]);
                    if (!amt.HasValue || amt.Value == 0m) continue;

                    var eg = result[m];

                    eg.Lines.Add(new SalesQuotationEgressLinDto
                    {
                        TempLineNo = tempLineNoNullable,
                        LineNo = eg.Lines.Count + 1,
                        MonthNo = m,
                        Amount = amt.Value
                    });
                }
            }

            // 6) Recalcular totales desde detalle (si hay detalle)
            foreach (var eg in result)
            {
                if (eg.Lines.Count > 0)
                    eg.Amount = eg.Lines.Sum(x => x.Amount);
            }

            return result;
        }

        // ==========================================================
        // ERRORES
        // ==========================================================

        private static void ValidateHeader(IXLWorksheet ws)
        {
            string[] expectedLabels =
            {
                "CLIENTE",
                "RUC",
                "DIRECCIÓN",
                "CONTACTO",
                "TELEFONO",
                "PROYECTO"
            };

            // Rango razonable donde vive la cabecera (ajústalo si tu template cambia)
            int minRow = 8;
            int maxRow = 20;

            foreach (var label in expectedLabels)
            {
                // 1) Buscar label en el rango (sin importar si la fila está oculta)
                var cell = ws.CellsUsed(XLCellsUsedOptions.All)
                    .Where(c => c.Address.RowNumber >= minRow && c.Address.RowNumber <= maxRow)
                    .FirstOrDefault(c =>
                    {
                        var s = (c.GetString() ?? "").Trim();
                        s = s.Replace(":", "").Trim();
                        return s.Equals(label, StringComparison.OrdinalIgnoreCase);
                    });

                if (cell == null)
                    throw new AppValidationException($"Formato inválido: falta el campo '{label}' en la cabecera.");

                // 2) Buscar el valor en la MISMA fila, hacia la derecha.
                // OJO: si hay merges, el valor puede estar varias columnas después.
                var row = cell.Address.RowNumber;
                var fromCol = cell.Address.ColumnNumber + 1;
                var toCol = ws.LastColumnUsed()?.ColumnNumber() ?? 50;

                // En vez de Row(row).Cells(...) (que a veces da vacío por merges),
                // buscamos por coordenadas directo.
                IXLCell? valueCell = null;
                for (int col = fromCol; col <= toCol; col++)
                {
                    var c = ws.Cell(row, col);

                    // Si está mergeado, lee la celda "master"
                    if (c.IsMerged())
                        c = c.MergedRange().FirstCell();

                    var v = (c.GetString() ?? "").Trim();
                    if (!string.IsNullOrWhiteSpace(v))
                    {
                        valueCell = c;
                        break;
                    }
                }

                if (valueCell == null)
                    throw new AppValidationException($"Formato inválido: el campo '{label}' está vacío.");
            }
        }
      
        private static void ValidateHeaderOrder(IXLWorksheet ws)
        {
            // AltText: valor alternativo aceptado (null = solo se acepta Text)
            var expected = new (string Text, int ColSpan, bool AllowEmpty, string? AltText)[]
            {
                ("ITEM",        1, false, null),
                ("DESCRIPCIÓN", 2, false, null),
                ("TIPO",        1, false, null),
                ("MED.",        1, false, null),
                ("MARCA",       1, false, null),
                ("MODELO",      1, false, null),
                ("CANT.",       1, false, null),
                ("P.U",         1, false, null),
                ("P.T.",        1, false, "DIA"),  // Variante A = P.T. | Variante B = DIA

                ("", 1, true, null),               // columna vacía separadora
                ("P.V.T.", 1, false, null),
            };

            int minRow = 15, maxRow = 35;

            var itemCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .Where(c => c.Address.RowNumber >= minRow && c.Address.RowNumber <= maxRow)
                .FirstOrDefault(c => Norm(c.GetString()) == "ITEM");

            if (itemCell == null)
                throw new AppValidationException("Formato inválido: no se encontró la cabecera 'ITEM'.");

            int row = itemCell.Address.RowNumber;
            int col = itemCell.Address.ColumnNumber;

            foreach (var (text, span, allowEmpty, altText) in expected)
            {
                var c = ws.Cell(row, col);

                string actualText;
                int actualSpan;

                if (c.IsMerged())
                {
                    var mr = c.MergedRange();
                    actualSpan = mr.ColumnCount();
                    actualText = Norm(mr.FirstCell().GetString());
                }
                else
                {
                    actualSpan = 1;
                    actualText = Norm(c.GetString());
                }

                if (actualSpan != span)
                    throw new AppValidationException($"Formato inválido: la columna '{text}' debería ocupar {span} y ocupa {actualSpan}.");

                if (allowEmpty)
                {
                    if (!string.IsNullOrWhiteSpace(actualText))
                        throw new AppValidationException($"Formato inválido: se esperaba una columna vacía antes de 'P.V.T.' y se encontró '{actualText}'.");
                }
                else
                {
                    bool matches = actualText == Norm(text)
                                || (altText != null && actualText == Norm(altText));
                    if (!matches)
                        throw new AppValidationException($"Formato inválido: se esperaba '{text}' y se encontró '{actualText}'.");
                }

                col += actualSpan;
            }

            static string Norm(string? s)
            {
                s = (s ?? "").Trim().Replace(":", "");
                s = string.Join(" ", s.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                return s.ToUpperInvariant();
            }
        }
        private static bool IsDayQuotationVariant(IXLWorksheet ws)
        {
            int minRow = 15, maxRow = 35;

            var itemCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .Where(c => c.Address.RowNumber >= minRow && c.Address.RowNumber <= maxRow)
                .FirstOrDefault(c => Norm(c.GetString()) == "ITEM");

            if (itemCell == null)
                return false;

            var diaCell = ws.Cell(itemCell.Address.RowNumber, itemCell.Address.ColumnNumber + 9);
            var text = diaCell.IsMerged()
                ? diaCell.MergedRange().FirstCell().GetString()
                : diaCell.GetString();

            return Norm(text) == "DIA";

            static string Norm(string? s)
            {
                s = (s ?? "").Trim().Replace(":", "");
                s = string.Join(" ", s.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                return s.ToUpperInvariant();
            }
        }
        private static void ValidateVvcAndResumenSection(IXLWorksheet ws)
        {
            // 1) Encontrar RESUMEN
            var resumenCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim()
                    .Equals("RESUMEN", StringComparison.OrdinalIgnoreCase));

            if (resumenCell == null)
                throw new AppValidationException("Formato inválido: no se encontró la sección 'RESUMEN'.");

            int resumenRow = resumenCell.Address.RowNumber;

            // 2) Validar que exista V.V.C. arriba (misma fila o fila anterior, por si está mergeado)
            bool hasVvc =
                ws.Row(resumenRow - 1).CellsUsed(XLCellsUsedOptions.All)
                    .Any(c => c.GetString().Trim().Equals("V.V.C.", StringComparison.OrdinalIgnoreCase))
                || ws.Row(resumenRow).CellsUsed(XLCellsUsedOptions.All)
                    .Any(c => c.GetString().Trim().Equals("V.V.C.", StringComparison.OrdinalIgnoreCase));

            if (!hasVvc)
                throw new AppValidationException("Formato inválido: falta el campo 'V.V.C.' en el bloque superior a RESUMEN.");

            // 3) Validar filas del resumen
            string[] expected = { "V. TOTAL", "C. TOTAL", "U. TOTAL", "M. TOTAL" };

            for (int i = 0; i < expected.Length; i++)
            {
                int row = resumenRow + 1 + i;

                bool found = ws.Row(row).CellsUsed(XLCellsUsedOptions.All)
                    .Any(c => c.GetString().Trim().Equals(expected[i], StringComparison.OrdinalIgnoreCase));

                if (!found)
                    throw new AppValidationException($"Formato inválido: en 'RESUMEN' falta la fila '{expected[i]}'.");
            }
        }
        private static void ValidateSeedCapitalSection(IXLWorksheet ws)
        {
            // 1) Buscar la celda "SEED CAPITAL"
            var seedCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim()
                    .Equals("SEED CAPITAL", StringComparison.OrdinalIgnoreCase));

            if (seedCell == null)
                throw new AppValidationException("Formato inválido: no se encontró la sección 'SEED CAPITAL'.");

            int row = seedCell.Address.RowNumber;
            int col = seedCell.Address.ColumnNumber;

            // 2) Validar los headers MES 01..MES 11 a la derecha del SEED CAPITAL
            for (int i = 1; i <= 11; i++)
            {
                string expected = $"MES {i:00}";
                var value = ws.Cell(row, col + i).GetString().Trim().ToUpperInvariant();

                if (value != expected)
                    throw new AppValidationException($"Formato inválido: se esperaba '{expected}' y se encontró '{value}'.");
            }

            // 3) (Opcional) Validar OUTLAY exista en alguna parte (o debajo)
            var outlayCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim()
                    .Equals("OUTLAY", StringComparison.OrdinalIgnoreCase));

            if (outlayCell == null)
                throw new AppValidationException("Formato inválido: no se encontró la fila 'OUTLAY'.");

            // 4) (Opcional) Validar botón/label "EGRESOS"
            var egresosCell = ws.CellsUsed(XLCellsUsedOptions.All)
                .FirstOrDefault(c => c.GetString().Trim()
                    .Equals("EGRESOS", StringComparison.OrdinalIgnoreCase));

            if (egresosCell == null)
                throw new AppValidationException("Formato inválido: no se encontró el label 'EGRESOS'.");
        }
        private static bool IsBrokenRef(Exception ex)
        {
            var msg = ex.Message ?? "";
            return msg.Contains("#REF!", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("wasn't parsed correctly", StringComparison.OrdinalIgnoreCase);
        }
        private static AppValidationException MapToExcelParsingException(Exception ex)
        {
            // Mensajes humanos y decentes
            if (ex is InvalidCastException)
                return new AppValidationException(
                    "El Excel tiene un dato con tipo inválido (ej: texto donde va número o fecha). Revisa montos y fechas.",
                    ex
                );

            if (ex is FormatException)
                return new AppValidationException(
                    "El Excel tiene un formato inválido (número o fecha). Revisa el contenido y vuelve a intentar.",
                    ex
                );

            // fallback
            return new AppValidationException(
                "No se pudo leer el Excel. Asegúrate de usar la plantilla correcta y que no esté corrupta.",
                ex
            );
        }

    }
}
