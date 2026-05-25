using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products
{
    public class ProductsResponseDto
    {
        public long ProductsId { get; set; }
        public long BusinessId { get; set; }
        public string? Description { get; set; }
        public string Sku { get; set; } = null!;
        public string CategoriesDescription { get; set; } = null!;
        public string LinesDescription { get; set; } = null!;
        public string TypesDescription { get; set; } = null!;
        public string BrandsDescription { get; set; } = null!;
        public decimal StockMin { get; set; }
        public bool IsActive { get; set; }
        public bool IsStockable { get; set; }
        public bool IsServices { get; set; }
        public bool IsReturnable { get; set; }
        public bool IsTool { get; set; }
        public bool CanBuy { get; set; }
        public bool CanSell { get; set; }
        public bool ManageLots { get; set; }
        public bool ManegesSerials { get; set; }
        public string Status { get; set; } = null!;
    }
}
