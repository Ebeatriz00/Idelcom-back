using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Orders
{
    public class QualitySupervisorCreateDto
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public long WorkerId { get; set; }
        public long UsersBy { get; set; }
    }
}
