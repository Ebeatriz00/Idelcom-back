using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClientsActivity
{
    public class ClientActivityUpdateDto
    {
        public long ClientsActivityId { get; set; }
        public long ActivityStateId { get; set; }
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
    }
}
