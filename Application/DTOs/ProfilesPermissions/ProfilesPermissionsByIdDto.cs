using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProfilesPermissions
{
    public class ProfilesPermissionsByIdDto
    {
        public long ProfilesPermissionsId { get; set; }
        public long BusinessId { get; set; }
        public long ProfilesId { get; set; }
        public long ModulesPermissionsId { get; set; }
    }
}
