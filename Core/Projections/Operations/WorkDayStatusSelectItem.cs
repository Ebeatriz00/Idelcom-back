using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Operations
{
    public class WorkDayStatusSelectItem
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
