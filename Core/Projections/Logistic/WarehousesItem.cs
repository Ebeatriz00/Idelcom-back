using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Logistic
{
    public class WarehousesItem
    {
        public long WarehousesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string DepartmentDescription { get; set; } = null!;
        public string ProvinceDescription { get; set; } = null!;
        public string DistrictDescription { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
