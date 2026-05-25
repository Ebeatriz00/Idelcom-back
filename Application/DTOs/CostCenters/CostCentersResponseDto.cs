using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CostCenters
{
    public class CostCentersResponseDto
    {
        public long CostCentersId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CostCentersCount { get; set; }
        public string Status { get; set; } = "1";
    }
}
