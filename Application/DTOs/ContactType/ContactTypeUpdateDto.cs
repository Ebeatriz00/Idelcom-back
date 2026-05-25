using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ContactType
{
    public class ContactTypeUpdateDto
    {
        public long ContactTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
