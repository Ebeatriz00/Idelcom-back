using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class InventoryStock : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long InventoryStockId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Almacén")]
        public long WarehouseId { get; set; }

        [AuditField("Cantidad Stock")]
        public decimal StockQuantity { get; set; }

        [AuditField("Cantidad Reservada")]
        public decimal ReservedQuantity { get; set; }

        [AuditField("Cantidad Disponible")]
        public decimal AvailableQuantity { get; set; }

        [AuditField("Costo promedio")]
        public decimal AverageCost { get; set; }

        [AuditField("Último costo")]
        public decimal LastCost { get; set; }

        [AuditField("Fecha de última entrada")]
        public DateTime? LastEnteryDate { get; set; }

        [AuditField("Fecha de última salida")]
        public DateTime? LastOutputDate { get; set; }

    }
}
