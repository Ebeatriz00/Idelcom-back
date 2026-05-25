using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductTypes
{
    public class ProductTypesCreateDto
    {
        public string Description { get; set; }
        public bool IsConsumable { get; set; }
        public bool IsReturnable { get; set; }
        public bool RequiresSerial { get; set; }
    }
}
