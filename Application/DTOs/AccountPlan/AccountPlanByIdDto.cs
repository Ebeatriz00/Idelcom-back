using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AccountPlan
{
    public class AccountPlanByIdDto
    {
        public long BusinessId { get; set; }
        public long AccountPlanId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int? AccountTypeId { get; set; }
        public int? accountLevelId { get; set; }
        public int? TypeAnalysisId { get; set; }
        public long? CurrencyId { get; set; }
        public int? AuxiliaryTypeId { get; set; }
        public string? DiferenceChange { get; set; }
        public string? DocControl { get; set; }
        public long? AccountAmarreDebit { get; set; }
        public long? AccountAmarreCredit { get; set; }
    }
}
