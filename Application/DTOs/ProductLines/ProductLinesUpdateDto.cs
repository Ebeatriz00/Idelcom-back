using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductLines
{
    public class ProductLinesUpdateDto
    {
        public long ProductLinesId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long CategoriesId { get; set; }
    }
}
