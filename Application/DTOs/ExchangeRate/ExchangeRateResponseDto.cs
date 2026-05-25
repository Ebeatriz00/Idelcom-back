using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExchangeRate
{
    public class ExchangeRateResponseDto
    {
        public long ExchangeRateId { get; set; }
        public long BusinessId { get; set; }
        public decimal PurchaseType { get; set; }
        public decimal SaleType { get; set; }
        public DateTime DateFxrate { get; set; } = DateTime.Now;
        public long ExchangeRateCount { get; set;}
        public string Status { get; set; } = "1"; 
    }
}
