using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PMVis
    {
        public long PMVisId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } 
        public string Status { get; set; } = "1";
        public long UsersBy {  get; set; }
        public int PMVisCount {  get; set; }
        
    }
}
