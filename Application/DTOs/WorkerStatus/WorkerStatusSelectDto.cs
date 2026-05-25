using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WorkerStatus
{
    public class WorkerStatusSelectDto
    {
        public long WorkerStatusId { get; set; }
        public string WorkerStatusDesc { get; set; } = string.Empty;
    }
}
