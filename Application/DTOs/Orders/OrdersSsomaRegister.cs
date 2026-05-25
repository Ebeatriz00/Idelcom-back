using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Orders
{
    public class OrdersSsomaRegister
    {
        public long OperationsId { get; set; }
        public long BusinessId { get; set; }
        public bool RequeredSsoma { get; set; }
        public List<long> WorkerId { get; set; } = new List<long>();
        public long UsersBy { get; set; }
    }
}
