using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class JobTitle
    {
        public long JobTitleId { get; set; }
        public long BusinessId { get; set; }
        public long AreaId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public string AreaDescription { get; set; } = string.Empty;
        public int JobTitleCount { get; set; }  
    }
}
