using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsSources
{
    public class LeadsSourcesStatusToggleDto
    {
        public long LeadsSourcesId { get; set; }
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}
