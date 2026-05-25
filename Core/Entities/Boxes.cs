using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Boxes
    {
        public long BoxesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long CurrencyId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public int BoxesCount { get; set; }
        public string CurrencyDescription { get; set; }

    }
}
