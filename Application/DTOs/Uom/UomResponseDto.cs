using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Uom
{
        public class UomResponseDto
        {
            public long UomId { get; set; }
            public long BusinessId { get; set; }
            public string CodeSunat { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int UomCount { get; set; }
            public string Status { get; set; } = "1";
        }
    }
