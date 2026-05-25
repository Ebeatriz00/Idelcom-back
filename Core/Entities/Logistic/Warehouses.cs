using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class Warehouses : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long WarehousesId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Almacén")]
        public string Description { get; set; } = null!;

        [AuditField("Dirección")]
        public string Address { get; set; } = null!;

        [AuditField("Departamento")]
        public long DepartmentId { get; set; }

        [AuditField("Provincia")]
        public long ProvinceId { get; set; }

        [AuditField("Distrito")]
        public long DistrictId { get; set; }


    }
}
