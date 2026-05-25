using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TaxAffType
{
    public class TaxAffTypeCreateDto
    {
        public long BusinessId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int UsersBy { get; set; }
    }
}
