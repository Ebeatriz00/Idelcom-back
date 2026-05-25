using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesResponseDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string? TypeOpporDesc { get; set; }
        public string NegotiationStagesDesc { get; set; }
        public string OpporDesc { get; set; }
        public string OpporNumber { get; set; }
        public int? PorcentProgressPro { get; set; }
        public string ClientsName { get; set; }
        public string? SalesName { get; set; }
        public string? SalesPre { get; set; }
        public int Tasks { get; set; }
        public int? DeliverablesCount { get; set; }
        public string StateOpporDesc { get; set; }
        public string? StateGeneral { get; set; }
        public string? ColorState { get; set; }
        public string StateColor { get; set; }
        public int? CommentsCount { get; set; }
        public int? unreadCommentsCount { get; set; }
        public string Status { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateFinish { get; set; }
        public long? IsOpporManager { get; set; }
        public bool? FollowupEnabled { get; set; }
        public DateTime? FollowupNextAt { get; set; }
        public bool? FollowupSuspended { get; set; }
        public bool? IsHiring { get; set; }

        /*===================== VALIDACIONES ========================*/
        public int? ObsNotResolved { get; set; }
        public int? ObsNotApproved { get; set; }
        public int? ObsApproved { get; set; }
        public int? ObsNotDate { get; set; }

        // Flag preventa
        public string? StatePresales { get; set; }
        public string? colorStatePresales { get; set; }
        public int? ObsNotResolvedPre { get; set; }
        public int? ObsNotApprovedPre { get; set; }
        public int? ObsApprovedPre { get; set; }
        public int? ObsNotDatePre { get; set; }
        public int? ObsQuo { get; set; }
        public int? ObsQuoResolved { get; set; }
        public int? ExistQuo { get; set; }
        public int? GoesToPreSales { get; set; }
        public int? PreSalesDelivered { get; set; }

        // Flag contrataciones
        public int? ObsNotResolvedLic { get; set; }
        public int? ObsNotApprovedLic { get; set; }
        public int? ObsApprovedLic { get; set; }
        public int? ObsNotDateLic { get; set; }
        public int? LicDocDelivered { get; set; }
        public int? LicConsultDelivered { get; set; }
        public int? CanChangeStateByDelivery { get; set; }

        /*===================== RESULTADOS ========================*/
        public int? TypeObsClientsId { get; set; }
        public int? TypeObsEconomic { get; set; }

        public decimal? TotalAmount { get; set; }
        public string? CurrencyDesc { get; set; }
        public DateTime? QuoDate { get; set; }
    }
}
