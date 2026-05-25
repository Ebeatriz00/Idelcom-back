using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Viability
{
    public class ViabilityStatusToggleDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string Status { get; set; }
        public long UsersBy { get; set; }
    }
}
