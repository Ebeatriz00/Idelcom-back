using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProfilesPermissions
{
    public class ProfilesPermissionsStatusToggleDto
    {
        public long ProfilesPermissionsId { get; set; }
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}
