using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ContactsCrm
{
    public class ContactsCrmCreateDto
    {
        public long BusinessId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public long WorkerId { get; set; }
        public long ClientsId { get; set; }
        public long LeadsSourcesId { get; set; }
        public long ContactTypeId { get; set; }
        public long UsersBy { get; set; }
    }
}
