using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductLines
{
    public class ProductLinesStatusToggleDto
    {
        public long ProductLinesId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = null!;

    }
}
