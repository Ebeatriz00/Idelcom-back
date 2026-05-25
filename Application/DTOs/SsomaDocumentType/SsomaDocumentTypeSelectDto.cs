using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SomaDocumentType
{
    public class SsomaDocumentTypeSelectDto
    {
        public long SsomaDocumentTypeId { get; set; }
        public string SsomaDocumentTypeDesc { get; set; } = string.Empty;
    }
}
