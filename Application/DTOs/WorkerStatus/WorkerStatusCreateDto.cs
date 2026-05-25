using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WorkerStatus
{
    public class WorkerStatusCreateDto
    {
        public long BusinessId { get; set; }
        public string WorkerStatusDesc { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
