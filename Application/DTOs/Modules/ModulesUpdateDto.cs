using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Modules
{
    public class ModulesUpdateDto
    {
        public long ModulesId { get; set; }
        public long BusinessId { get; set; }
        public long ParentModulesId { get; set; }
        public long ParentId { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string ModulesDescription { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public string UsersBy { get; set; }
    }
}

