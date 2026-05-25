using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Business
{
    public class BusinessViewDto
    {
        public string BusinessRuc { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string BusinessName { get; set; }
        public string AboutBusiness { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public bool IsVerified { get; set; }
        public string BusinessPhone { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessLegalRepre { get; set; } = string.Empty;
        public string LegalDocument { get; set; } = string.Empty;
        public string LegalFirm { get; set; } = string.Empty;
        public string RetentionAgent { get; set; } = string.Empty;
        public string PerceptionAgent { get; set; } = string.Empty;
        public string Pricos { get; set; } = string.Empty;
        public string FileRuc { get; set; } = string.Empty;
        public string FileComplanceCertificate { get; set; } = string.Empty;
        public string BusinessLogo { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;

        public List<AddressBusinessDto> AddressBusiness { get; set; } = new();
    }
    public class AddressBusinessDto
    {
        public long Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public bool MainAddress { get; set; } = false;
        public string Address { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
    }
}
