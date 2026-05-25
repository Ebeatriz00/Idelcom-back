using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ConceptGroups
{
    public class ConceptGroupsSelectDto
    {
        public long ConceptGroupsId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
