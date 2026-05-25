using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class AccountByIdDto
    {
        public long AccountId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long BankId { get; set; }
        public long CurrencyId { get; set; }
        public long AccountPlanId { get; set; }
    }
}
