using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ModulePermission
{
    public class ModulesPermissionsResponseDto
    {
            public long ModulesPermissionsId { get; set; }
            public string ModulesName { get; set; }
            public string ModulesDescription { get; set; }
            public int TotalPermissions { get; set; }
            public int ActivePermissions { get; set; }
            public int UsedInProfiles { get; set; }
            public string Status { get; set; } = "1";
            public List<ListPermissionsDto> ListModulesPermissions { get; set; } = new();
    }

    public class ListPermissionsDto
    {
        public long ModulesPermissionsId { get; set; }
        public long PermissionsId { get; set; }
        public string PermissionsName { get; set; }

    }

}
