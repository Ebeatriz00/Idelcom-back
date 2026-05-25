using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Uom
{
    public class UomSelectDto
    {
        public long UomId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
