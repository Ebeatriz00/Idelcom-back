using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExchangeRate
{
    public class ExchangeRateSelectDto
    {
        public long ExchangeRateId { get; set; }
        public DateTime DateFxRate { get; set; } = DateTime.Now;
    }
}
