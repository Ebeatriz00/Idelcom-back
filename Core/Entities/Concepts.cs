using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Concepts
    {
        public long ConceptsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long ConceptGroupsId { get; set; }
        public long AccountPlanId { get; set; }
        public long UsersBy {  get; set; }
        public string Status { get; set; } = "1";
        public int ConceptsCount { get; set; }
        public string ConceptGroupsDescription { get; set; }
        public string AccountPlanDescription { get; set; }
    }
}
