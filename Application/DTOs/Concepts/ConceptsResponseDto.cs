using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Concepts
{
    public class ConceptsResponseDto
    {
        public long ConceptsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public string ConceptGroupsDescription { get; set; } = string.Empty;
        public string AccountPlanDescription { get; set; } = string.Empty;
        public int ConceptsCount { get; set; }
    }
}
