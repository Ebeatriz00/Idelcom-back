using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobTitle
{
    public class JobTitleStatusToggleDto
    {
        public long JobTitleId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public int Usersby { get; set; }
    }
}
