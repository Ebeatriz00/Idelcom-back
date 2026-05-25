using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductLines
{
    public class ProductLinesCreateDto
    {
        public string Description { get; set; } = string.Empty;
        public long CategoriesId { get; set; }
    }
}
