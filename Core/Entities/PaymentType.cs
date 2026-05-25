using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PaymentType
    {
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public int PaymentCount { get; set; }

    }
}
