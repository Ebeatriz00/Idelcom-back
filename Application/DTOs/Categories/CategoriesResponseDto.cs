using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Categories
{
    public class CategoriesResponseDto
    {
        public long CategoriesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
