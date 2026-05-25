using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periods
{
    public class PeriodsBlockToggleDto
    {
        public long PeriodsId { get; set; }
        public long BusinessId { get; set; }
        public bool IndBlock { get; set; }
        public long UsersBy { get; set; }
    }
}
