using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class PurchaseReceipt : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long PurchaseReceiptId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Orden de compra")]
        public long? PurchaseOrderId { get; set; }

        [AuditField("Proveedor")]
        public long SuppliersId { get; set; }

        [AuditField("Almacen")]
        public long WarehouseId { get; set; }

        [AuditField("Tipo de recepcion")]
        public long ReceiptTypeId { get; set; } = 1;

        [AuditField("N° de recepcion")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [AuditField("Fecha de recepcion")]
        public DateTime ReceiptDate { get; set; }

        [AuditField("Guia de proveedor")]
        public string? SupplierGuideNumber { get; set; } 

        [AuditField("Fecha de guia")]
        public DateTime? SupplierGuideDate { get; set; }

        [AuditField("Movimiento")]
        public long? WarehouseMovementId { get; set; }

        [AuditField("Estado de recepcion")]
        public long ReceiptStatusId { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Sin orden de compra")]
        public bool IsWithoutPurchaseOrder { get; set; }

        [AuditField("Regularizada")]
        public bool IsRegularized { get; set; }

        [AuditField("Orden de compra regularizada")]
        public long? RegularizedPurchaseOrderId { get; set; }

        [AuditField("Fecha regularizacion")]
        public DateTime? RegularizedDate { get; set; }

        [AuditField("Usuario regularizacion")]
        public long? RegularizedUser { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}
