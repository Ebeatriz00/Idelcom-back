using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaMovementType
{
    public class SsomaMovementTypeStatusToogleDto
    {
        public long SsomaMovementTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; }
        public long UsersBy { get; set; }
    }
}
