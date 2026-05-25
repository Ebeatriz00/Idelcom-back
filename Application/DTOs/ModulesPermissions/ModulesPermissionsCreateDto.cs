using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ModulePermission
{
    public class ModulesPermissionsCreateDto
    {
        public long BusinessId { get; set; }
        public long ModulesId { get; set; }
        public int PermissionsId { get; set; }
        public int UsersBy { get; set; }
    }
}
