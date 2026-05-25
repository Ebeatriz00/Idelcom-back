using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ModulePermission
{
    public class ModulesPermissionsStatusToggleDto
    {
            public long ModulesPermissionsId { get; set; }
            public long BusinessId { get; set; }
            public long UsersBy { get; set; }
            public string Status { get; set; } = "1";
        }
    }
