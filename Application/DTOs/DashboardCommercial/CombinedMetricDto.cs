using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dashboard
{
    public class CombinedMetricDto
    {
        public int QuarterNum { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }
}
