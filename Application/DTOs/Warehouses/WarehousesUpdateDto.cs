using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Warehouses
{
    public class WarehousesUpdateDto
    {
        public long WarehousesId { get; set; }
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public long DepartmentId { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
    }
}
