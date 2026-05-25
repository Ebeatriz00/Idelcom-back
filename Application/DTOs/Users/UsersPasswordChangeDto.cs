using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users
{
    public class UsersPasswordChangeDto
    {
        public long BusinessId { get; set; }
        public long UsersId { get; set; }
        public string UsersPassword { get; set; } = string.Empty;
        public int UsersBy { get; set; }
    }
}
