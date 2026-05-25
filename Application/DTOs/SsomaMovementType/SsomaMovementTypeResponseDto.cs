using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaMovementType
{
    public class SsomaMovementTypeResponseDto
    {
        public long SsomaMovementTypeId { get; set; }
        public long BusinessId { get; set; }
        public string SsomaMovementTypeDesc { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
