using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Area
{
    internal class AreaSelectDto
   {
     public long AreaId { get; set; }
     public string Description { get; set; } = string.Empty;
    }
}
