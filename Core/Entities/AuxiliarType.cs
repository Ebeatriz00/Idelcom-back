using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AuxiliarType
    {
        public long AuxiliarTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}
