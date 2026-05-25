using Application.DTOs.PurchaseOrder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Services
{
    public class PurchaseOrderPdfDocument(PurchaseOrderPdfData data) : IDocument
    {
        // ── Idelcom Design System ────────────────────────────────────────────────
        private static readonly string PrimaryBlue     = "#003779";
        private static readonly string ContainerBlue   = "#0B4DA1";
        private static readonly string SecondaryOrange = "#9A4600";
        private static readonly string OrangeContainer = "#FE8028";
        private static readonly string Surface         = "#F9F9FF";
        private static readonly string SurfaceLow      = "#F0F3FF";
        private static readonly string SurfaceContainer= "#E7EEFF";
        private static readonly string OnSurface       = "#111C2D";
        private static readonly string OnSurfaceVar    = "#424752";
        private static readonly string OutlineVariant  = "#C3C6D3";
        private static readonly string White           = "#FFFFFF";

        // Fonts (mapped to QuestPDF available fonts)
        private const string FontHeadline = "Arial";
        private const string FontBody     = "Segoe UI";
        private const string FontMono     = "Courier New";

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public DocumentSettings GetSettings() => new()
        {
            PdfA = false,
            ImageCompressionQuality = ImageCompressionQuality.Medium,
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(42);  // ~1.5 cm
                page.MarginVertical(34);    // ~1.2 cm
                page.PageColor(Colors.White);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposePage1Content);
                page.Footer().Element(ComposeFooter);
            });

            if (data.Items.Count > 10)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(42);
                    page.MarginVertical(34);
                    page.PageColor(Colors.White);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeSignaturesPage);
                    page.Footer().Element(ComposeFooter);
                });
            }
        }

        // ── HEADER ──────────────────────────────────────────────────────────────
        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Left: company info
                    row.RelativeItem(3).Column(c =>
                    {
                        c.Item().Text(data.Company.Name)
                            .FontFamily(FontHeadline).Bold().FontSize(11)
                            .FontColor(OnSurface);
                        c.Item().Text($"RUC: {data.Company.TaxId}")
                            .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                        c.Item().Text($"Dir: {data.Company.Address}")
                            .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                        if (!string.IsNullOrWhiteSpace(data.Company.Phone))
                            c.Item().Text($"Tel: {data.Company.Phone}")
                                .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                    });

                    // Center: logo
                    row.RelativeItem(2).AlignCenter().AlignMiddle().Column(c =>
                    {
                        if (data.Company.LogoBytes is not null)
                            c.Item().AlignCenter().MaxHeight(48).Image(data.Company.LogoBytes).FitWidth();
                        else
                            c.Item().AlignCenter().Text("IDELCOM")
                                .FontFamily(FontHeadline).Bold().FontSize(14).FontColor(PrimaryBlue);
                    });

                    // Right: document title + number
                    row.RelativeItem(3).AlignRight().Column(c =>
                    {
                        c.Item().Text("ORDEN DE COMPRA")
                            .FontFamily(FontHeadline).Bold().FontSize(16).FontColor(OnSurface);
                        c.Item().Text($"N°: {data.OrderNumber}")
                            .FontFamily(FontHeadline).Bold().FontSize(13).FontColor(SecondaryOrange);
                    });
                });

                col.Item().PaddingTop(6).BorderBottom(2).BorderColor(ContainerBlue);
            });
        }

        // ── PAGE 1 CONTENT ──────────────────────────────────────────────────────
        private void ComposePage1Content(IContainer container)
        {
            container.PaddingTop(8).Column(col =>
            {
                // Supplier + Order info block
                col.Item().Border(1).BorderColor(OutlineVariant).CornerRadius(8)
                    .Background(Surface).Padding(10).Row(row =>
                {
                    // Left: supplier
                    row.RelativeItem(3).Column(c =>
                    {
                        c.Item().Text("PROVEEDOR")
                            .FontFamily(FontHeadline).Bold().FontSize(8).FontColor(PrimaryBlue);
                        c.Item().PaddingTop(4).Element(cnt => InfoPair(cnt, "Razón Social", data.Supplier.Name));
                        c.Item().Element(cnt => InfoPair(cnt, "RUC", data.Supplier.TaxId));
                        if (!string.IsNullOrWhiteSpace(data.Supplier.Address))
                            c.Item().Element(cnt => InfoPair(cnt, "Dirección", data.Supplier.Address));
                        if (!string.IsNullOrWhiteSpace(data.Supplier.Phone))
                            c.Item().Element(cnt => InfoPair(cnt, "Teléfono", data.Supplier.Phone));
                        if (!string.IsNullOrWhiteSpace(data.Supplier.Contact))
                            c.Item().Element(cnt => InfoPair(cnt, "Contacto", data.Supplier.Contact));
                        if (!string.IsNullOrWhiteSpace(data.Supplier.Email))
                            c.Item().Element(cnt => InfoPair(cnt, "Correo", data.Supplier.Email));
                        if (!string.IsNullOrWhiteSpace(data.SupplierQuotationNumber))
                            c.Item().Element(cnt => InfoPair(cnt, "Cotización N°", data.SupplierQuotationNumber));
                    });

                    // Separator
                    row.ConstantItem(1).Background(OutlineVariant);

                    // Right: order data
                    row.RelativeItem(2).PaddingLeft(8).Column(c =>
                    {
                        c.Item().Text("DATOS DE LA ORDEN")
                            .FontFamily(FontHeadline).Bold().FontSize(8).FontColor(PrimaryBlue);
                        c.Item().PaddingTop(4).Element(cnt => InfoPair(cnt, "Fecha Emisión",
                            data.IssueDate.ToString("dd/MM/yyyy")));
                        c.Item().Element(cnt => InfoPair(cnt, "Fecha Entrega",
                            data.DeliveryDate.HasValue ? data.DeliveryDate.Value.ToString("dd/MM/yyyy") : "—"));
                        if (!string.IsNullOrWhiteSpace(data.PaymentCondition))
                            c.Item().Element(cnt => InfoPair(cnt, "Forma de Pago", data.PaymentCondition));
                        if (!string.IsNullOrWhiteSpace(data.Currency))
                            c.Item().Element(cnt => InfoPair(cnt, "Moneda", data.Currency));
                        if (data.ExchangeRate.HasValue && data.ExchangeRate > 0)
                            c.Item().Element(cnt => InfoPair(cnt, "Tipo de Cambio",
                                data.ExchangeRate.Value.ToString("N4")));
                    });
                });

                // Reference bar
                if (!string.IsNullOrWhiteSpace(data.Reference))
                {
                    col.Item().PaddingTop(6)
                        .Background(SurfaceContainer).CornerRadius(4).Padding(6).Row(r =>
                    {
                        r.AutoItem().Text("REFERENCIA:")
                            .FontFamily(FontBody).Bold().FontSize(8).FontColor(PrimaryBlue);
                        r.AutoItem().PaddingLeft(4).Text(data.Reference)
                            .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                    });
                }

                // Items table
                col.Item().PaddingTop(8).Element(ComposeItemsTable);

                // Totals block
                col.Item().PaddingTop(8).AlignRight().Width(220).Element(ComposeTotals);

                // Notes
                if (!string.IsNullOrWhiteSpace(data.Notes))
                {
                    col.Item().PaddingTop(8).Column(c =>
                    {
                        c.Item().Text("NOTAS")
                            .FontFamily(FontHeadline).Bold().FontSize(8).FontColor(PrimaryBlue);
                        c.Item().PaddingTop(2)
                            .Background(SurfaceContainer).CornerRadius(4).Padding(6)
                            .Text(data.Notes).FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                    });
                }

                // Bill To / Ship To
                if (data.BillTo is not null || data.ShipTo is not null)
                {
                    col.Item().PaddingTop(20).Row(row =>
                    {
                        if (data.BillTo is not null)
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("FACTURAR A")
                                    .FontFamily(FontHeadline).Bold().FontSize(8).FontColor(PrimaryBlue);
                                c.Item().Element(cnt => InfoPair(cnt, "Nombre", data.BillTo.Name));
                                if (!string.IsNullOrWhiteSpace(data.BillTo.TaxId))
                                    c.Item().Element(cnt => InfoPair(cnt, "RUC", data.BillTo.TaxId));
                                if (!string.IsNullOrWhiteSpace(data.BillTo.Address))
                                    c.Item().Element(cnt => InfoPair(cnt, "Dirección", data.BillTo.Address));
                            });
                        }

                        if (data.ShipTo is not null)
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DESPACHAR A")
                                    .FontFamily(FontHeadline).Bold().FontSize(8).FontColor(PrimaryBlue);
                                c.Item().Element(cnt => InfoPair(cnt, "Almacén", data.ShipTo.Name));
                                if (!string.IsNullOrWhiteSpace(data.ShipTo.TaxId))
                                    c.Item().Element(cnt => InfoPair(cnt, "RUC", data.ShipTo.TaxId));
                                if (!string.IsNullOrWhiteSpace(data.ShipTo.Address))
                                    c.Item().Element(cnt => InfoPair(cnt, "Dirección", data.ShipTo.Address));
                            });
                        }
                    });
                }

                // Signatures inline if items fit on one page
                if (data.Items.Count <= 10)
                    col.Item().PaddingTop(36).Element(ComposeSignatureBlocks);
            });
        }

        // ── ITEMS TABLE ─────────────────────────────────────────────────────────
        private void ComposeItemsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(18);  // #
                    cols.ConstantColumn(48);  // Código
                    cols.RelativeColumn();    // Descripción
                    cols.ConstantColumn(26);  // UM
                    cols.ConstantColumn(32);  // Cant.
                    cols.ConstantColumn(52);  // P. Unit.
                    cols.ConstantColumn(52);  // Importe
                });

                // Header row
                table.Header(header =>
                {
                    void HeaderCell(string text, HorizontalAlignment align = HorizontalAlignment.Center)
                    {
                        header.Cell().Background(ContainerBlue).Padding(3)
                            .AlignCenter()
                            .Text(text).FontFamily(FontBody).Bold().FontSize(8).FontColor(White);
                    }

                    HeaderCell("#");
                    HeaderCell("Código");
                    HeaderCell("Descripción", HorizontalAlignment.Left);
                    HeaderCell("UM");
                    HeaderCell("Cant.");
                    HeaderCell($"P. Unit. {data.CurrencySymbol ?? "S/"}");
                    HeaderCell($"Importe {data.CurrencySymbol ?? "S/"}");
                });

                // Data rows
                for (int i = 0; i < data.Items.Count; i++)
                {
                    var item = data.Items[i];
                    var bg = i % 2 == 0 ? White : SurfaceLow;

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(2)
                        .AlignCenter()
                        .Text(item.Number.ToString()).FontFamily(FontBody).FontSize(8).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(2)
                        .AlignCenter()
                        .Text(item.Code ?? string.Empty).FontFamily(FontMono).FontSize(7.5f).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(3)
                        .Text(item.Description).FontFamily(FontBody).FontSize(8).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3)
                        .AlignCenter()
                        .Text(item.Um ?? string.Empty).FontFamily(FontBody).FontSize(8).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(2)
                        .AlignRight()
                        .Text(item.Quantity.ToString("N2")).FontFamily(FontMono).FontSize(8).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(2)
                        .AlignRight()
                        .Text(item.UnitPrice.ToString("N2")).FontFamily(FontMono).FontSize(8).FontColor(OnSurface);

                    table.Cell().BorderBottom(0.5f).BorderColor(OutlineVariant).Background(bg)
                        .PaddingVertical(3).PaddingHorizontal(2)
                        .AlignRight()
                        .Text(item.Amount.ToString("N2")).FontFamily(FontMono).FontSize(8).FontColor(OnSurface);
                }
            });
        }

        // ── TOTALS ──────────────────────────────────────────────────────────────
        private void ComposeTotals(IContainer container)
        {
            var symbol   = data.CurrencySymbol ?? "S/";
            var subtotal = data.Items.Sum(i => i.Amount);
            var igv      = Math.Round(subtotal * 0.18m, 2);
            var total    = subtotal + igv;

            container.Column(col =>
            {
                // SUB TOTAL
                col.Item().BorderBottom(0.5f).BorderColor(OutlineVariant)
                    .Background(Surface).PaddingVertical(4).PaddingHorizontal(6).Row(r =>
                {
                    r.RelativeItem().Text("SUB TOTAL")
                        .FontFamily(FontBody).Bold().FontSize(8).FontColor(OnSurface);
                    r.AutoItem().Text($"{symbol} {subtotal:N2}")
                        .FontFamily(FontMono).FontSize(8).FontColor(OnSurface);
                });

                // IGV 18%
                col.Item().BorderBottom(0.5f).BorderColor(OutlineVariant)
                    .Background(Surface).PaddingVertical(4).PaddingHorizontal(6).Row(r =>
                {
                    r.RelativeItem().Text("IGV 18%")
                        .FontFamily(FontBody).Bold().FontSize(8).FontColor(OnSurface);
                    r.AutoItem().Text($"{symbol} {igv:N2}")
                        .FontFamily(FontMono).FontSize(8).FontColor(OnSurface);
                });

                // TOTAL
                col.Item().Background(PrimaryBlue)
                    .PaddingVertical(5).PaddingHorizontal(6).Row(r =>
                {
                    r.RelativeItem().Text("TOTAL")
                        .FontFamily(FontBody).Bold().FontSize(9).FontColor(White);
                    r.AutoItem().Text($"{symbol} {total:N2}")
                        .FontFamily(FontMono).Bold().FontSize(10).FontColor(White);
                });
            });
        }

        // ── SIGNATURES PAGE (page 2 or inline) ─────────────────────────────────
        private void ComposeSignaturesPage(IContainer container)
        {
            container.PaddingTop(30).Element(ComposeSignatureBlocks);
        }

        private void ComposeSignatureBlocks(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().AlignCenter().Text(data.PurchaseManager ?? "___________________")
                        .FontFamily(FontHeadline).Bold().FontSize(9).FontColor(OnSurface);
                    c.Item().BorderBottom(1).BorderColor(OutlineVariant).Height(1);
                    c.Item().PaddingTop(3).AlignCenter().Text("Encargado de Compras")
                        .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                });

                row.ConstantItem(40);

                row.RelativeItem().Column(c =>
                {
                    c.Item().AlignCenter().Text(data.Approver ?? "___________________")
                        .FontFamily(FontHeadline).Bold().FontSize(9).FontColor(OnSurface);
                    c.Item().BorderBottom(1).BorderColor(OutlineVariant).Height(1);
                    c.Item().PaddingTop(3).AlignCenter().Text("Aprobado por")
                        .FontFamily(FontBody).FontSize(8).FontColor(OnSurface);
                });
            });
        }

        // ── FOOTER ──────────────────────────────────────────────────────────────
        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().BorderTop(1.5f).BorderColor(ContainerBlue).Height(1);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text(data.Company.Name)
                        .FontFamily(FontBody).FontSize(7).FontColor("#737783");
                    row.AutoItem().Text(text =>
                    {
                        text.Span("Página ").FontFamily(FontBody).FontSize(7).FontColor("#737783");
                        text.CurrentPageNumber().FontFamily(FontBody).FontSize(7).FontColor("#737783");
                        text.Span(" de ").FontFamily(FontBody).FontSize(7).FontColor("#737783");
                        text.TotalPages().FontFamily(FontBody).FontSize(7).FontColor("#737783");
                    });
                });
            });
        }

        // ── HELPERS ─────────────────────────────────────────────────────────────
        private static void InfoPair(IContainer container, string label, string? value)
        {
            container.PaddingBottom(2).Row(r =>
            {
                r.ConstantItem(72).Text(label + ":")
                    .FontFamily(FontMono).Bold().FontSize(7.5f).FontColor(OnSurface);
                r.RelativeItem().Text(value ?? "—")
                    .FontFamily(FontBody).SemiBold().FontSize(7.5f).FontColor(OnSurface);
            });
        }

        static PurchaseOrderPdfDocument()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ── STATIC GENERATORS ───────────────────────────────────────────────────
        public static void GeneratePdf(PurchaseOrderPdfData data, string filePath)
        {
            new PurchaseOrderPdfDocument(data).GeneratePdf(filePath);
        }

        public static MemoryStream GeneratePdfStream(PurchaseOrderPdfData data)
        {
            var ms = new MemoryStream();
            new PurchaseOrderPdfDocument(data).GeneratePdf(ms);
            return ms;
        }
    }
}
