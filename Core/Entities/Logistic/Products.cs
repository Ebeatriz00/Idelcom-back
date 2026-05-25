using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class Products :  BaseAuditableEntity
    {
        [AuditField("Id")]
        public long ProductsId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("SKU")]
        public string Sku { get; set; } = null!;

        [AuditField("Código de Barra")]
        public string Barcode { get; set; } = null!;

        [AuditField("Número de partida")]
        public string PartNum { get; set; } = null!;

        [AuditField("Descripción")]
        public string Description { get; set; } = null!;

        [AuditField("Descripción Corta")]
        public string ShortDescription { get; set; } = null!;

        [AuditField("Tipo de producto")]
        public long ProductTypeId { get; set; }

        [AuditField("Línea de producto")]
        public long ProductLinesId { get; set; }

        [AuditField("Categoría")]
        public long CategoriesId { get; set; }

        [AuditField("Marca")]
        public long BrandsId { get; set; }

        [AuditField("Unidad de Medida")]
        public long UomId { get; set; }

        [AuditField("Sunat Código")]
        public long SunatId { get; set; }

        [AuditField("Stock Mínimo")]
        public decimal StockMin { get; set; }

        [AuditField("Stock Máximo")]
        public decimal StockMax { get; set; }

        [AuditField("Factor de Conversión")]
        public decimal ConversionFactor { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }

        [AuditField("Stockable")]
        public bool IsStockable { get; set; }

        [AuditField("Servicios")]
        public bool IsServices { get; set; }

        [AuditField("Retornable")]
        public bool IsReturnable { get; set; }

        [AuditField("Herramienta")]
        public bool IsTool { get; set; }

        [AuditField("Puede Comprar")]
        public bool CanBuy { get; set; }

        [AuditField("Puede Vender")]
        public bool CanSell { get; set; }

        [AuditField("Gestionar Lotes")]
        public bool ManageLots { get; set; }

        [AuditField("Gestionar Series")]
        public bool ManegesSerials { get; set; }

        [AuditField("Control de Vencimiento")]
        public bool ExpirationControl { get; set; }

        [AuditField("Peso")]
        public decimal Weight { get; set; }

        [AuditField("Volumen")]
        public decimal Volume { get; set; }

        [AuditField("Archivos")]
        public string Files { get; set; } = null!;
    }
}
