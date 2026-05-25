using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DocumentType
{
    public class DocumentTypeUpdateDto
    {
        public long DocumentTypeId { get; set; }
        public int BusinessId { get; set; }
        public string CodeSunat { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UsersBy { get; set; }
    }
}
