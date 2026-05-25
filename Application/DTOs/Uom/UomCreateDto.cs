using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Uom
{
        public class UomCreateDto
        {
            public long BusinessId { get; set; }
            public string CodeSunat { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int UsersBy { get; set; }
        }
    }
