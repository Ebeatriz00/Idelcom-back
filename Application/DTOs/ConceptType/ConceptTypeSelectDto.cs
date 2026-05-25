using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ConceptType
{
    public class ConceptTypeSelectDto
    {
        public long ConceptTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
