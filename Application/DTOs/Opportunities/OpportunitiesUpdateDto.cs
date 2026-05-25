using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesUpdateDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }

        public int TypeOppor { get; set; }
        public long ParentOpporId { get; set; }

        public long NegotiationStagesId { get; set; }
        public string OpporDesc { get; set; }
        public long ClientsId { get; set; }
        public long? ContactsId { get; set; }
        public long? BusinessLineId { get; set; }
        public long? StateOpporId { get; set; }
        public long? WorkerId { get; set; }
        public long? CurrencyId { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateFinish { get; set; }
        public Decimal? OpporAmount { get; set; }
        public DateTime? ConsultDate { get; set; }
        public DateTime? QuoDate { get; set; }
        public int? PorcentProgressPro { get; set; }
        public long UsersBy { get; set; }

        public bool? FollowupEnabled { get; set; }
        public int? FollowupEveryDay { get; set; }
        public bool? IsHiring { get; set; }
        public long? FlowTypeId { get; set; }
        public long? PmConditionId { get; set; }
        public List<DeliverablesOpporDto>? DeliverablesHiring { get; set; }
        public List<OpportFiletrackingDto>? HiringFiles { get; set; }
    }
}
