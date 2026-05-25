using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovementTypes
{
    public class MovementTypesSelectDto
    {
        public long MovementTypesId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
