using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Concepts
{
    public class ConceptsByIdDto
    {
        public long ConceptsId { get; set; }
        public long BusinessId { get; set; }  
        public string Description { get; set; } = string.Empty;
        public long ConceptGroupsId { get; set; }
        public long AccountPlanId { get; set; }
    }
}
