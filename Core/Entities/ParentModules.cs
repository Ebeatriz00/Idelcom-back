using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ParentModules
    {
        public long ParentModulesId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string? IconKey { get; set; }
        public string StickyBottom { get; set; }
        public int OrderNo { get; set; }
        public int ParentCount { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}
