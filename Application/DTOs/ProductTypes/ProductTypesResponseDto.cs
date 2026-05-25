using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductTypes
{
    public class ProductTypesResponseDto
    {
        public long ProductTypesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string? IsConsumable { get; set; }
        public string? IsReturnable { get; set; }
        public string? RequiresSerial { get; set; }
        public string? Status { get; set; } 
    }
}
