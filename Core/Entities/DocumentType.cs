using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentType
    {
        public long DocumentTypeId { get; set; }
        public long BusinessId { get; set; }
        public string CodeSunat { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int UsersBy { get; set; } 
        public int DocumentTypeCount { get; set; } 
        public string Abrv { get; set; } = string.Empty;
    }
}
