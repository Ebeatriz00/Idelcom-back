using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentModules
{
    public class ParentModulesStatusToogleDto
    {
        public long ParentModulesId { get; set; }
        public long BusinessId { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; }
    }
}
