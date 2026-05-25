using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products
{
    public class ProductsStatusToggleDto
    {
        public long ProductsId { get; set; }
        public string Status { get; set; } = null!;
    }
}
