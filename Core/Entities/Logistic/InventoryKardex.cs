using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class InventoryKardex : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long InventoryKardexId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Almacén")]
        public long WarehouseId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Movimiento Almacén")]
        public long WareHouseMovementId { get; set; }

        [AuditField("Movimiento Detalle Almacén")]
        public long WareHouseMovementDetailId { get; set; }

        [AuditField("Tipo de movimiento")]
        public long MovementTypesId { get; set; }

        [AuditField("Fecha de Movimiento")]
        public DateTime MovementDate { get; set; }

        [AuditField("Cantidad de Entrada")]
        public decimal EntryQuantity { get; set; }

        [AuditField("Cantidad de salida")]
        public decimal ExitQuantity { get; set; }

        [AuditField("Stock Previo")]
        public decimal PreviousStock { get; set; }

        [AuditField("Stock Final")]
        public decimal FinalStock { get; set; }

        [AuditField("Costo unitario")]
        public decimal UnitCost { get; set; }

        [AuditField("Costo promedio")]
        public decimal AverageCost { get; set; }

        [AuditField("Costo total")]
        public decimal TotalCost { get; set; }

        [AuditField("Tipo de documento de ref")]
        public string? ReferenceDocumentType { get; set; }

        [AuditField("N° de referencia")]
        public string? ReferenceDocumentNumber { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }


    }
}
