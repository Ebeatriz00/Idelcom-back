using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Warehouses
{
    public class WarehousesStatusToggleDto
    {
        public long WarehousesId { get; set; }
        public string Status { get; set; } = null!;
    }
}
