using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CostCenters
{
    public class CostCentersSelectDto
    {
        public long CostCentersId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
