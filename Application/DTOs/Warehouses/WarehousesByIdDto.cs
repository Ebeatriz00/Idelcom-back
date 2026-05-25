using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Warehouses
{
    public class WarehousesByIdDto
    {
        public long WarehousesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public long DepartmentId { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
    }
}
