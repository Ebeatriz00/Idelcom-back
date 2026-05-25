using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Boxes
{
    public class BoxesCreateDto
    {
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long CurrencyId { get; set; }
        public long UsersBy { get; set; }
    }
}
