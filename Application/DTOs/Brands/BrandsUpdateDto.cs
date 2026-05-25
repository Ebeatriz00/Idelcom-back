using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Brands
{
    public class BrandsUpdateDto
    {
        public long BrandsId { get; set; }
        public string Description { get; set; } = null!;
    }
}
