using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ModulesPermissions
    {
        public long ModulesPermissionsId { get; set; }
        public long BusinessId { get; set; }
        public long ModulesId { get; set; }
        public long PermissionsId { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";


        public string ModulesName { get; set; }
        public string ModulesDescription { get; set; }
        public int TotalPermissions { get; set; }
        public int ActivePermissions { get; set; }
        public int UsedInProfiles { get; set; }

        public List<ListPermissions> ListModulesPermissions { get; set; } = new();
    }

    public class ListPermissions
    {
        public long ModulesPermissionsId { get; set; }
        public long PermissionsId { get; set; }
        public string PermissionsName { get; set; }

    }
}
