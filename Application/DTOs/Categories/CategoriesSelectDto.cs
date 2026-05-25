using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Categories
{
    public class CategoriesSelectDto
    {
        public long CategoriesId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
