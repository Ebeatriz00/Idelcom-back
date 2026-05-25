using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ModulePermission
{
    public class ModulesPermissionsUpdateDto
    {
            public long ModulesPermissionsId { get; set; }
            public long BusinessId { get; set; }
            public long ModulesId { get; set; }
            public long PermissionsId { get; set; }
            public long UsersBy { get; set; }
    }

}
