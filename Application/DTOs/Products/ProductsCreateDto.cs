
using Application.DTOs.FileTrackingProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products
{
    public class ProductsCreateDto
    {
        public string Sku { get; set; } = null!;        
        public string Barcode { get; set; } = null!;        
        public string PartNum { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public long ProductTypeId { get; set; }
        public long ProductLinesId { get; set; }
        public long CategoriesId { get; set; }
        public long BrandsId { get; set; }
        public long UomId { get; set; }
        public long SunatId { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
        public decimal ConversionFactor { get; set; }
        public bool IsActive { get; set; }
        public bool IsStockable { get; set; }
        public bool IsServices { get; set; }
        public bool IsReturnable { get; set; }
        public bool IsTool { get; set; }
        public bool CanBuy { get; set; }
        public bool CanSell { get; set; }
        public bool ManageLots { get; set; }
        public bool ManegesSerials { get; set; }
        public bool ExpirationControl { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public List<FileTrackingProductsCreateDto> Files { get; set; } = new();
    }
}
