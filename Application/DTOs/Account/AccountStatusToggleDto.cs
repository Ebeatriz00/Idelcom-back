using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class AccountStatusToggleDto
    {
        public long AccountId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
    }
}
