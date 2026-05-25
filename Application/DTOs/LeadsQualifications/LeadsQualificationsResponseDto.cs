using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsQualifications
{
    public class LeadsQualificationsResponseDto
    {
        public long LeadsQualificationsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int LeadsQualificationsCount { get; set; }
    }
}
