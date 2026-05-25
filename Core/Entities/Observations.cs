using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Observations
    {
        public long? ObsId { get; set; }
        public long? BusinessId { get; set; }
        public long? OpporId { get; set; }
        public int? ObsType { get; set; }
        public int? ObsSeverity { get; set; }
        public long? ObsStatusId { get; set; }
        public string? ObsReason { get; set; }
        public DateTime? DueDate { get; set; }
        public long? DueSetBy { get; set; }
        public long? OpenedBy { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? Status { get; set; }
        public string? ObsSeverityDesc { get; set; }
        public string? OpenedByName { get; set; }
        public string? DueSetByName { get; set; }
        public string OpporToken { get; set; }
        public string? RejectionReason { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? StateColor { get; set; }
        public string? StateDesc { get; set; }
        public Boolean? IsApproved { get; set; }
        public int? TypeObsEconomic { get; set; }
        public long? UsersBy { get; set; }
        public Boolean? AffectsQuotation { get; set; }
    }
}

