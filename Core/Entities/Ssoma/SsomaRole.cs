using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Ssoma
{
    public class SsomaRole : BaseAuditableEntity
    {
        public int SsomaRoleId {get; set;}
        public long BusinessId { get; set; }
        public string SsomaRoleDesc { get; set; }
    }
}
