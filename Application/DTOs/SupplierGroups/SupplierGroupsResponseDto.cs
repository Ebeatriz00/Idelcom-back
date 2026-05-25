using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SupplierGroups
{
    public class SupplierGroupsResponseDto
    {
        public long SupplierGroupsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SupplierGroupsCount { get; set; }
        public string Status { get; set; } = "1";
    }
}
