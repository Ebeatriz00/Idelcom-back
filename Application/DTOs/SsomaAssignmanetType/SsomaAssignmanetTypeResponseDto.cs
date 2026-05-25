using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaAssignmanetType
{
    public class SsomaAssignmanetTypeResponseDto
    {
        public int SsomaAssignamentTypeId { get; set; }
        public string SsomaAssignamentName { get; set; } = null!;
        public long Status { get; set; }
    }
}
