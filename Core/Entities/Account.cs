using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Account
    {
        public long AccountId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long BankId { get; set; }
        public long CurrencyId { get; set; }
        public long AccountPlanId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public int AccountCount { get; set; }
        public string BankDescription { get; set; }
        public string CurrencyDescription { get; set; }
        public string AccountPlanDescription { get; set; }
    }
}
