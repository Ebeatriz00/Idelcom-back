using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsQualifications
{
    public class LeadsQualificationsSelectDto
    {
        public long LeadsQualificationsId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
