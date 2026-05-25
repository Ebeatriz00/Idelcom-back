using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ContactsCrm
{
    public class ContactsCrmResponseDto
    {
        public long ContactsCrmId { get; set; }
        public long BusinessId { get; set; }
        public string? ContactName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? Movil { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; } = "1";
        public string? WorkerDescritpion { get; set; } = string.Empty;
        public string? ClientsDescription { get; set; } = string.Empty;
        public string? LeadsSourcesDescription { get; set; } = string.Empty;
        public string? ContactTypeDescription { get; set; } = string.Empty;
        public int ContactsCrmCount { get; set; }

    }
}
