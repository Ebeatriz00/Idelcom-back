using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sector
{
    public class SectorResponseDto
    {
        public long SectorId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SectorCount { get; set; }
        public string Status { get; set; }
    }
}
