using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProfilesPermissions
{
    public class ProfilesPermissionsResponseDto
    {
        public long ProfilesPermissionsId { get; set; }
        public string ModulesName { get; set; }
        public string PermissionsName { get; set; }
        public string Status { get; set; }
    }
}
