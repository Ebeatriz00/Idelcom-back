using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AccountPlan
{
    public class AccountPlanStatusToggleDto
    {
        public long BusinessId { get; set; }
        public long AccountPlanId { get; set; }
        public long UsersBy { get; set; }
        public string? Status { get; set; }
    }
}
