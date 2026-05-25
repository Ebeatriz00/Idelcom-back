using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class PurchaseOrder : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long PurchaseOrderId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Proveedor")]
        public long SuppliersId { get; set; }

        [AuditField("N° Orden de compra")]
        public string PurchaseOrderNumber { get; set; } = null!;

        [AuditField("Fecha Orden de compra")]
        public DateTime PurchaseOrderDate { get; set; }

        [AuditField("Moneda")]
        public long CurrencyId { get; set; }

        [AuditField("Tipo de cambio")]
        public decimal? ExchangeRate { get; set; }

        [AuditField("Condición de pago")]
        public long? PmConditionId { get; set; }

        [AuditField("Fecha de entrega prevista")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [AuditField("Almacén")]
        public long? WarehouseId { get; set; }

        [AuditField("Nro referencia cotizacion proveedor")]
        public string? SupplierQuotationReferenceNumber { get; set; }

        [AuditField("Referencias")]
        public string? References { get; set; }

        [AuditField("SubTotal")]
        public decimal Subtotal { get; set; }

        [AuditField("Descuento")]
        public decimal DiscountAmount { get; set; }

        [AuditField("Impuesto")]
        public decimal TaxAmount { get; set; }

        [AuditField("Total")]
        public decimal Total { get; set; }

        [AuditField("Estado de Orden de compra")]
        public long PurchaseOrderStatusId { get; set; }

        [AuditField("Regularizacion")]
        public bool IsRegularization { get; set; }

        [AuditField("Motivo regularizacion")]
        public string? RegularizationReason { get; set; }

        [AuditField("Regularizado por")]
        public long? RegularizedBy { get; set; }

        [AuditField("Fecha regularizacion")]
        public DateTime? RegularizationDate { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Solicitado por")]
        public long? RequestedBy { get; set; }

        [AuditField("Aprodabo por")]
        public long? ApprovedBy { get; set; }

        [AuditField("Fecha de aprobación")]
        public DateTime? ApprovedAt { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}
