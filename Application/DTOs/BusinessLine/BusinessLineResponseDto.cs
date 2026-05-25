using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BusinessLine
{
    public class BusinessLineResponseDto
    {
        public int BusinessLineId { get; set; }
        public long BusinessId { get; set; }
        public string DescLine { get; set; }
        public string Status { get; set; }
    }
}
