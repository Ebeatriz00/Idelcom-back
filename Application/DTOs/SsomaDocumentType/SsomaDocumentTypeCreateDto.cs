using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SomaDocumentType
{
    public class SsomaDocumentTypeCreateDto
    {
        public long BusinessId { get; set; }
        public string SsomaDocumentTypeDesc { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
