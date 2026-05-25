using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesClientsGetByIdDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long ClientsId { get; set; }
        public string? ReasonRejection { get; set; }
    }
}
