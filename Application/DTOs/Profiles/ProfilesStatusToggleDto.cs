using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Profiles
{
    public class ProfilesStatusToggleDto
    {
        public long ProfilesId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public int UsersBy { get; set; }
    }
}
