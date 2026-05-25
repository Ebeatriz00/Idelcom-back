using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobTitle
{
    public class JobTitleResponseDto
    {
        public long JobTitleId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public string AreaDescription { get; set; } = string.Empty;
        public int JobTitleCount { get; set; }
    }
}
