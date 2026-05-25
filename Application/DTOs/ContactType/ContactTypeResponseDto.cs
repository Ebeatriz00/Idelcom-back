using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ContactType
{
    public class ContactTypeResponseDto
    {
        public long ContactTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int ContactTypeCount { get; set; }
    }
}
