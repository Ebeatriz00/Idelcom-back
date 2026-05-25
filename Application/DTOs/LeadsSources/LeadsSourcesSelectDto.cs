using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsSources
{
    public class LeadsSourcesSelectDto
    {
        public long LeadsSourcesId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
