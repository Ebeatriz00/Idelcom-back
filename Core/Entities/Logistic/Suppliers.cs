using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class Suppliers : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SuppliersId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Tipo de proveedor")]
        public long SupplierTypeId { get; set; }

        [AuditField("Grupo de proveedor")] 
        public long SupplierGroupsId { get; set; }

        [AuditField("Condición de pago")] 
        public long PaymentConditionId { get; set; }

        [AuditField("Metódo de pago")] 
        public long PaymentMethodId { get; set; }

        [AuditField("Tipo de documento")] 
        public long DocumentTypeId { get; set; }

        [AuditField("Número de documento")] 
        public string? DocumentNumber { get; set; }

        [AuditField("Razón social")] 
        public string? SupplierName { get; set; }

        [AuditField("Nombre comercial")] 
        public string? TradeName { get; set; }

        [AuditField("Contacto")]
        public string? ContactName { get; set; }

        [AuditField("Dirección")]
        public string? Address { get; set; }

        [AuditField("Departamento")]
        public long? DepartmentId { get; set; }

        [AuditField("Provincia")]
        public long? ProvinceId { get; set; }

        [AuditField("Distrito")]
        public long? DistrictId { get; set; }

        [AuditField("Teléfono")]
        public string? Phone { get; set; }

        [AuditField("Celular")]
        public string? Mobile { get; set; }

        [AuditField("Correo")]
        public string? Email { get; set; }

        [AuditField("Página web")]
        public string? Website { get; set; }

        [AuditField("Agente retenedor")]
        public bool RetainerAgent { get; set; }

        [AuditField("Agente de percepción")]
        public bool PerceptionAgent { get; set; }

        [AuditField("Agente de detracción")]
        public bool DetractionAgent { get; set; }

        [AuditField("Agente extranjero")]
        public bool ForeignAgent { get; set; }

        [AuditField("Observacion")]
        public string? Observation {  get; set; }
    }
}
