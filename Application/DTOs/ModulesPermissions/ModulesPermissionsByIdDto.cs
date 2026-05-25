using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ModulesPermissions
{
    public class ModulesPermissionsByIdDto
    {
        public long ModulesPermissionsId { get; set; }
        public long ModulesId { get; set; }
        public long PermissionsId { get; set; }

    }
}
