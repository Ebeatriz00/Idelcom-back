using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PMCondition
{
    public class PMConditionResponseDto
    {
        public long PMConditionId { get; set; }
        public long BussinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public int PmConditionCount { get; set; }
    }
}
