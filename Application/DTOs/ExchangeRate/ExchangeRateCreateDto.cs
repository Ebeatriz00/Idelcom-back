using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExchangeRate
{
    public class ExchangeRateCreateDto
    {
        public long BusinessId { get; set; }
        public decimal PurchaseType { get; set; }
        public decimal SaleType { get; set; }
        public DateTime DateFxrate { get; set; }
        public long UsersBy { get; set; }
    }
}
