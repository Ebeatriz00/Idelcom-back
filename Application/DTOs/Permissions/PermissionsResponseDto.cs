using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Permissions
{
    public class PermissionsResponseDto
    {
        public long PermissionsId { get; set; }
        public long BusinessId { get; set; }
        public string? PermissionsCode { get; set; }
        public string PermissionsName { get; set; }
        public string PermissionsDescription { get; set; } = string.Empty;
        public int PermissionsCount { get; set; }
        public string Status { get; set; } = "1";
    }
}
