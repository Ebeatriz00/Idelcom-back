using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClientsActivity
{
    public class ClientActivityCreateDto
    {
        public long BusinessId { get; set; }
        public long ClientsId { get; set; }
        public long WorkerId { get; set; }

        public int ActivityTypeId { get; set; }
        public long ActivityStateId { get; set; }

        public DateTime FinishDate { get; set; }
        public string Description { get; set; }

        public long UsersBy { get; set; }
    }
}
