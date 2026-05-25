using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PriorityState
{
    public class PriorityStateCreateDto
    {
        public long BusinessId { get; set; }
        public string Color { get; set; }
        public string PriorityDesc { get; set; }
        public long UsersBy { get; set; }
    }
}
