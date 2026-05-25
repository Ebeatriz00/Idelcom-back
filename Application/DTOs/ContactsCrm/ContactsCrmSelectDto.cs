using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ContactsCrm
{
    public class ContactsCrmSelectDto
    {
        public long ContactsCrmId { get; set; }
        public string ContactName { get; set; } = string.Empty;

    }
}
