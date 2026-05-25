using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Boxes
{
    public class BoxesByIdDto
    {
        public long BoxesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long CurrencyId { get; set; }
    }
}
