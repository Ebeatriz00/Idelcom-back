using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Worker
{
    public class WorkerResponseDto
    {
        public long WorkerId { get; set; }
        public string WorkerFullName { get; set; }
        public string DocumentType { get; set; }
        public string? District { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string WorkerDocument { get; set; }
        public string? Address { get; set; }
        public DateTime? DateEntry { get; set; }
        public string Status { get; set; } = "1";
        public int WorkerCount { get; set; }
    }
}
