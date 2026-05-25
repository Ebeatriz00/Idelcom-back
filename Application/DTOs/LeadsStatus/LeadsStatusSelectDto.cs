using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeadsStatus
{
    public class LeadsStatusSelectDto
    {
        public long LeadsStatusId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
