using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OpporViability
{
    public class OpporViabilityConvertDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
