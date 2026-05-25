using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobTitle
{
    public class JobTitleByIdDto
    {
        public long JobTitleId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public long AreaId { get; set; }
    }
}
