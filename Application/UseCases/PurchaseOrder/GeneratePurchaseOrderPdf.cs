using Application.DTOs.PurchaseOrder;
using Application.Exceptions;
using Application.Services;
using Core.Interfaces.Logistic;
using SkiaSharp;
using Svg.Skia;

namespace Application.UseCases.PurchaseOrder
{
    public class GeneratePurchaseOrderPdf(IPurchaseOrderRepository repository)
    {
        private readonly IPurchaseOrderRepository _repository = repository;

        private static readonly Dictionary<string, byte[]> _logoCache = new();
        private static readonly object _logoCacheLock = new();

        public async Task<(MemoryStream Stream, string OrderNumber)> ExecuteAsync(long businessId, long purchaseOrderId)
        {
            var projection = await _repository.GetPdfDataAsync(businessId, purchaseOrderId);
            if (projection?.Header is null)
                throw new NotFoundException("Orden de compra", purchaseOrderId);

            var header = projection.Header;

            var logoBytes = await FetchLogoBytesAsync(header.CompanyLogoPath);

            var pdfData = new PurchaseOrderPdfData
            {
                Company = new CompanyInfo
                {
                    Name      = header.CompanyName ?? string.Empty,
                    TaxId     = header.CompanyTaxId ?? string.Empty,
                    Address   = header.CompanyAddress ?? string.Empty,
                    Phone     = header.CompanyPhone ?? string.Empty,
                    LogoBytes = logoBytes,
                },
                Supplier = new SupplierInfo
                {
                    Name    = header.SupplierName ?? string.Empty,
                    TaxId   = header.SupplierTaxId ?? string.Empty,
                    Address = header.SupplierAddress,
                    Phone   = header.SupplierPhone,
                    Contact = header.SupplierContact,
                    Email   = header.SupplierEmail,
                },
                OrderNumber             = header.PurchaseOrderNumber ?? string.Empty,
                IssueDate               = header.PurchaseOrderDate,
                DeliveryDate            = header.ExpectedDeliveryDate,
                PaymentCondition        = header.PaymentCondition,
                Currency                = header.CurrencyDescription,
                CurrencySymbol          = header.CurrencySymbol,
                ExchangeRate            = header.ExchangeRate,
                SupplierQuotationNumber = header.SupplierQuotationReferenceNumber,
                Reference               = header.References,
                Notes                   = header.Observation,
                Approver                = header.ApproverName,
                PurchaseManager         = header.PurchaseManagerName,
                BillTo = header.BillToName is not null ? new BillingAddress
                {
                    Name    = header.BillToName,
                    TaxId   = header.BillToTaxId,
                    Address = header.BillToAddress,
                } : null,
                ShipTo = header.ShipToName is not null ? new BillingAddress
                {
                    Name    = header.ShipToName,
                    TaxId   = header.ShipToTaxId,
                    Address = header.ShipToAddress,
                } : null,
                Items = projection.Details.Select((d, idx) => new OrderItem
                {
                    Number      = idx + 1,
                    Code        = d.ProductCode,
                    Description = d.ProductDescription ?? string.Empty,
                    Um          = d.UomDescription,
                    Quantity    = d.Quantity,
                    UnitPrice   = d.UnitPrice,
                    Amount      = d.Amount,
                }).ToList(),
            };

            var stream = await Task.Run(() => PurchaseOrderPdfDocument.GeneratePdfStream(pdfData));
            return (stream, pdfData.OrderNumber);
        }

        private static async Task<byte[]?> FetchLogoBytesAsync(string? logoPath)
        {
            if (string.IsNullOrWhiteSpace(logoPath)) return null;

            lock (_logoCacheLock)
            {
                if (_logoCache.TryGetValue(logoPath, out var cached))
                    return cached;
            }

            try
            {
                byte[] rawBytes;

                if (logoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    logoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    rawBytes = await http.GetByteArrayAsync(logoPath);
                }
                else if (File.Exists(logoPath))
                {
                    rawBytes = await File.ReadAllBytesAsync(logoPath);
                }
                else
                {
                    return null;
                }

                if (logoPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    var skSvg = new SKSvg();
                    using var stream = new MemoryStream(rawBytes);
                    skSvg.Load(stream);

                    if (skSvg.Picture is null) return null;

                    var bounds = skSvg.Picture.CullRect;
                    int w = Math.Max((int)bounds.Width, 1);
                    int h = Math.Max((int)bounds.Height, 1);

                    var imageInfo = new SKImageInfo(w, h, SKColorType.Rgba8888, SKAlphaType.Premul);
                    using var surface = SKSurface.Create(imageInfo);
                    surface.Canvas.Clear(SKColors.Transparent);
                    surface.Canvas.DrawPicture(skSvg.Picture);
                    surface.Canvas.Flush();

                    using var image = surface.Snapshot();
                    using var pngData = image.Encode(SKEncodedImageFormat.Png, 100);
                    rawBytes = pngData.ToArray();
                }

                lock (_logoCacheLock)
                    _logoCache[logoPath] = rawBytes;

                return rawBytes;
            }
            catch
            {
                return null;
            }
        }
    }
}
