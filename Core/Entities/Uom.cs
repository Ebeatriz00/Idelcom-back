using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Uom
    {
        public long UomId { get; set; }
        public long BusinessId { get; set; }
        public string CodeSunat { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int UomCount { get; set; }   
        public int UsersBy {  get; set; }    
    }
}
