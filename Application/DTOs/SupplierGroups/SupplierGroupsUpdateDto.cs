using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SupplierGroups
{
    public class SupplierGroupsUpdateDto
    {
        public long SupplierGroupsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
