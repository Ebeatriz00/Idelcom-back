using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductTypes
{
    public class ProductTypesSelectDto
    {
        public long ProductTypesId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
