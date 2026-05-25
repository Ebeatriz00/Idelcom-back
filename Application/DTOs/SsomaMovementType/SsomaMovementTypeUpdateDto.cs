using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaMovementType
{
    public class SsomaMovementTypeUpdateDto
    {
        public long SsomaMovementTypeId { get; set; }
        public long BusinessId { get; set; }
        public string SsomaMovementTypeDesc { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
