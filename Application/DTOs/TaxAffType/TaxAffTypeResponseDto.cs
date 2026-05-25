using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TaxAffType
{
    public class TaxAffTypeResponseDto
    {
        public long TaxAffTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public int TaxAffCount { get; set; }
    }
}
