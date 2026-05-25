using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Permissions
{
    internal class PermissionsSelectDto
    {
        public long PermissionsId { get; set; }
        public string PermissionsName { get; set; } = string.Empty;
    }
}
