using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ConceptType
{
    public class ConceptTypeStatusToggleDto
    {
        public long ConceptTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public int UsersBy { get; set; }
    }
}
