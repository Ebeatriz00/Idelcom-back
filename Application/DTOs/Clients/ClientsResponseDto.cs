using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Clients
{
    public class ClientsResponseDto
    {
        public long ClientsId { get; set; }
        public long BusinessId { get; set; }
        public string ClientsName { get; set; }
        public string Documents { get; set; }
        public string? Sales { get; set; }
        public string? Sector { get; set; }
        public string? Departament { get; set; }
        public string? LeadStatus { get; set; }
        public bool IsOtherSeller { get; set; }
        public string Status { get; set; }

    }
    public class ClientsHistoryResponseDto
    {
        public long? EventId { get; set; }
        public DateTime ChangeAt { get; set; }
        public string? ChangeUser { get; set; }
        public string? Description { get; set; }

    }
}
