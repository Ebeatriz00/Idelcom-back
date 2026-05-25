using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Concepts
{
    public class ConceptsUpdateDto
    {
        public long ConceptsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int ConceptGroupsId { get; set; }
        public int AccountPlanId { get; set; }
        public long UsersBy { get; set; }
    }
}
