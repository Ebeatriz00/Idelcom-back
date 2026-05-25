using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Brands
{
    public class BrandsResponseDto
    {
        public long BrandsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
