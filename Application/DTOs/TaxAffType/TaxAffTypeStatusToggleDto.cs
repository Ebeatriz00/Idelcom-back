using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TaxAffType
{
    public class TaxAffTypeStatusToggleDto
    {
        public long BusinessId { get; set; }
        public long TaxAffTypeId { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}
