using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ssoma
{
    public class SsomaAssignmanetTypeCreateDto
    {
        public long BusinessId { get; set; }
        public string SsomaAssignamentName { get; set; } = null!;
        public long UsersBy { get; set; }
    }
}
