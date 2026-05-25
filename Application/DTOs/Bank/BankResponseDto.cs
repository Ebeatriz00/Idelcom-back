using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Bank
{
    public class BankResponseDto
    {
        public long BankId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int BankCount { get; set; }
    }
}
