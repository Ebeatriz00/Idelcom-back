using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Currency
    {
        public long CurrencyId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CodeSunat { get; set; } = string.Empty;
        public string symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int CurrencyCount { get; set; }
        public int UsersBy { get; set; }

    }
}
