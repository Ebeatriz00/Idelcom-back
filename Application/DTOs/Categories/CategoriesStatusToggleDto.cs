using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Categories
{
    public class CategoriesStatusToggleDto
    {
        public long CategoriesId { get; set; }
        public string Status { get; set; }= null!;
    }
}
