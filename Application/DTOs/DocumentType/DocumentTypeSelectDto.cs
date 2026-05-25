using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DocumentType
{
    internal class DocumentTypeSelectDto
    {
        public  long DocumentTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
