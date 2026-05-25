using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ConceptGroups
{
    public class ConceptGroupsUpdateDto
    {
        public long ConceptGroupsId { get; set; }
        public long BusinessId { get; set; }
        public int ConceptTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
