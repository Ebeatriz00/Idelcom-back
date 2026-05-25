using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovVis
{
    public class MovVisResponseDto
    {
        public long MovVisId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public int MovVisCount { get; set; }
    }
}
