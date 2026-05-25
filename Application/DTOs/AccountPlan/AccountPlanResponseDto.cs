using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AccountPlan
{
    public class AccountPlanResponseDto
    {
        public long BusinessId { get; set; }
        public long AccountPlanId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public int AccountPlanCount { get; set; }
        public string? Status { get; set; }
    }
}
