using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   
    public class ConceptGroups
    {
        public long ConceptGroupsId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ConceptTypeId { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public int ConceptGroupCount { get; set; }
        public string ConceptTypeDescription {  get; set; }

    }
}
