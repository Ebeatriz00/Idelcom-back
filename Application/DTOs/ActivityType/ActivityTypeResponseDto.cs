using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivityType
{
    public class ActivityTypeResponseDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string? ActivityIcon { get; set; }
        public string? ActivityDesc { get; set; }
        public string Status { get; set; }
    }
}
