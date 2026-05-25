using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Viability
{
    public class ViabilityGetByIdDto
    {
        public string linkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public int Approved { get; set; }
        public int ViabilityDecision { get; set; }
        public string OpporToken { get; set; }

        public int PartialCompliance { get; set; }
        public int Compliance { get; set; }
        public int NonCompliance { get; set; }

        public int Authority { get; set; }
        public int Budget { get; set; }
        public int Need { get; set; }
        public int Term { get; set; }
        public int WorkExperience { get; set; }
        public int StaffExperience { get; set; }
        public int CompanyExperience { get; set; }
        public int Ability { get; set; }
        public int Schedule { get; set; }

        public string? AuthorityDesc { get; set; }
        public string? BudgetDesc { get; set; }
        public string? NeedDesc { get; set; }
        public string? TermDesc { get; set; }
        public string? WorkExpDesc { get; set; }
        public string? StaffExpDesc { get; set; }
        public string? CompanyExpDesc { get; set; }
        public string? AbilityDesc { get; set; }
        public string? ScheduleDesc { get; set; }
    }
}
