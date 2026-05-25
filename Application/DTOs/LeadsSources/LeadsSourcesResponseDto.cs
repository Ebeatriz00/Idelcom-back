using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsSources
{
    public class LeadsSourcesResponseDto
    {
        public long LeadsSourcesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int LeadsSourcesCount { get; set; }
        public string Status { get; set; }
    }
}
