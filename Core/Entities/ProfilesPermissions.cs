using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProfilesPermissions
    {
        public long ProfilesPermissionsId { get; set; }
        public long BusinessId { get; set; }
        public long ProfilesId { get; set; }
        public long ModulesPermissionsId { get; set; }
        public long UsersBy    { get; set; }
        public string Status { get; set; } = "1";

        //Listar

        public string ModulesName { get; set; }
        public string PermissionsName { get; set; }
    }
}
