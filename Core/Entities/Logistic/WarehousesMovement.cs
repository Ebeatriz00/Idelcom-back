using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class WarehousesMovement : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long WarehouseMovementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Tipo de Movimiento")]
        public long MovementTypeId { get; set; }

        [AuditField("Almacén")]
        public long WarehouseId { get; set; }

        [AuditField("Almacén Destino")]
        public long WarehouseDestinationId { get; set; }

        [AuditField("Proveedor")]
        public long SuppliersId { get; set; }

        [AuditField("Cliente")]
        public long ClientsId { get; set; }

        [AuditField("Serie")]
        public string? Series { get; set; }

        [AuditField("N° Documento")]
        public string? NumberDocument { get; set; }

        [AuditField("N° de referencia")]
        public string? ReferenceDocument { get; set; }

        [AuditField("Fecha de movimiento")]
        public DateTime? MovementDate { get; set; }

        [AuditField("observacion")]
        public string? Observation { get; set; }

        [AuditField("Impuesto")]
        public long? TaxesId { get; set; }

        [AuditField("Porcentaje IGV")]
        public decimal IgvPercent { get; set; }

        [AuditField("Subtotal")]
        public decimal  SubTotal { get; set; }

        [AuditField("Monto IGV")]
        public decimal Igv {  get; set; }

        [AuditField("Total")]
        public decimal Total { get; set; }

        [AuditField("OC")]
        public long? PurchaseOrderId { get; set; }

        [AuditField("Modulo de origen")]
        public string? SourceModule { get; set; }

        [AuditField("Documento Origen")]
        public long? SourceDocumentId { get; set; }

        [AuditField("Tipo Origen")]
        public string? SourceDocumentType { get; set; }

        [AuditField("Tipo Documento Origen")]
        public string? SourceDocumentTypeId { get; set; }

        [AuditField("Tipo Documento proveedor")]
        public long? SupplierDocumentTypeId { get; set; }

        [AuditField("Serie proveedor")]
        public string? SupplierSeries { get; set; }

        [AuditField("Nro documento proveedor")]
        public string? SupplierNumberDocument { get; set; }

        [AuditField("Fecha documento proveedor")]
        public DateTime? SupplierDocumentDate { get; set; }

        [AuditField("tipo de recido")]
        public long? ReceiptTypeId { get; set; }

        [AuditField("Parcialmente recibido")]
        public bool? IsPartialReceipt { get; set; }

        [AuditField("Recivido por")]
        public long? ReceivedBy { get; set; }

        [AuditField("Fecha de recibido")]
        public DateTime? ReceivedDate { get; set; }

    }
}
