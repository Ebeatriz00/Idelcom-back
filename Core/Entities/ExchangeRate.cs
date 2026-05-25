using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ExchangeRate
    {
        public long ExchangeRateId { get; set; }
        public long BusinessId { get; set; }
        public decimal PurchaseType { get; set; }
        public decimal SaleType { get; set; }
        public DateTime DateFxrate { get; set; } = DateTime.Now;
        public string Status { get; set; } 
        public int ExchangeRateCount { get; set; }
        public int UsersBy { get; set; }

    }
}
