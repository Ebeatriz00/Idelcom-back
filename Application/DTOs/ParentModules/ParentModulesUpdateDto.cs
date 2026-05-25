using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentModules
{
    public class ParentModulesUpdateDto
    {
        public long ParentModulesId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public bool StickyBottom { get; set; }
        public int OrderNo { get; set; }
        public int UsersBy { get; set; }
    }
}
