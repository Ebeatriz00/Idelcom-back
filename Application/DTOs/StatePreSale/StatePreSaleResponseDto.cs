using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StatePreSale
{
    public class StatePreSaleResponseDto
    {
        public long StatePreSaleId { get; set; }
        public long BusinessId { get; set; }
        public string StateDesc { get; set; }
        public int NumPercPro { get; set; }
        public string Status { get; set; }
    }
}
