using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaAssignmanetType
{
    public class SsomaAssignmanetTypeByIdDto
    {
        public int SsomaAssignamentTypeId { get; set; }
        public long BusinessId { get; set; }
        public string SsomaAssignamentName { get; set; } = null!;
    }
}
