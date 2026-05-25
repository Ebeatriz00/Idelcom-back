using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Business
{
    public class BusinessCreateDto
    {
        public string CodeLicense { get; set; }
        public string License { get; set; }
        public string BusinessRuc { get; set; }
        public DateTime BusinessStartDate { get; set; }
        public string BusinessExercise { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string BusinessName { get; set; }
        public string AboutBusiness { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public bool IsMainCompany { get; set; }
        public bool IsVerified { get; set; }
        public string BusinessAddress { get; set; } = string.Empty;
        public string BusinessCountry { get; set; } = string.Empty;
        public string Department { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string BusinessPhone { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessLegalRepre { get; set; } = string.Empty;
        public int LegalDocumentType { get; set; } = 0;
        public string LegalDocument { get; set; } = string.Empty;
        public string LegalFirm { get; set; } = string.Empty;
        public string FileRuc { get; set; } = string.Empty;
        public string FileComplianceCertificate { get; set; } = string.Empty;
        public int CurrencyMain { get; set; }
        public int CurrencySecondary { get; set; }
        public string BusinessLogo { get; set; } = string.Empty;
        public string RetentionAgent { get; set; } = string.Empty;
        public string PerceptionAgent { get; set; } = string.Empty;
        public string Pricos { get; set; } = string.Empty;
        public int UsersBy { get; set; }
    }
}
