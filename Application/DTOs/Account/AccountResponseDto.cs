using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class AccountResponseDto
    {
        public long AccountId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public string BankDescription { get; set; } = string.Empty;
        public string CurrencyDescription { get; set; } = string.Empty;
        public string AccountPlanDescription { get; set; } = string.Empty;
        public int AccountCount { get; set; }
    }
}
