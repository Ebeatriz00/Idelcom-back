using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProfilesPermissions
{
    public class ProfilesPermissionsCreateDto
    {
        public long BusinessId { get; set; }
        public long ProfilesId { get; set; }
        public List<long> ModulesPermissionsId { get; set; } = new();
        public long UsersBy { get; set; }
    }
}
