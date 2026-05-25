using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SsomaAssignmanetType
    {
        public int SsomaAssignamentTypeId { get; set; }
        public long BusinessId { get; set; }
        public string SsomaAssignamentName { get; set; } = null!;

        public string Status { get; set; }
        public long UsersBy {  get; set; }

    }
}
