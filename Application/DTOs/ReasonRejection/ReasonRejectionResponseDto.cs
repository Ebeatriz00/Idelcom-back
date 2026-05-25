using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReasonRejection
{
    public class ReasonRejectionResponseDto
    {
        public long ReasonRejectionId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; }
        public int ReasonRejectionCount { get; set; }

    }
}
