using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductTypes
{
    public class ProductTypesUpdateDto
    {
        public long ProductTypesId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsConsumable { get; set; }
        public bool IsReturnable { get; set; }
        public bool RequiresSerial { get; set; }
    }
}
