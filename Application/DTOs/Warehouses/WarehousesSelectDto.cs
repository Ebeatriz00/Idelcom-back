using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Warehouses
{
    public class WarehousesSelectDto
    {
        public long WarehousesId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
