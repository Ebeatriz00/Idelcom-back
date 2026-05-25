using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovementTypes
{
    public class MovementTypesStatusToggleDto
    {
        public long MovementTypesId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
    }
}
