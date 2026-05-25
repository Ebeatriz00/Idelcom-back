using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DocumentType
{
    public class DocumentTypeStatusToggleDto
    {
        public long DocumentTypeId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; } = "1";
        public int UsersBy { get; set; }
    }
}
