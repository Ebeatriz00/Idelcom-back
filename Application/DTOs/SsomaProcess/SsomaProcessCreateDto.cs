using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SsomaProcess
{
    public class SsomaProcessCreateDto
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public bool RequiresCompanyHomologation { get; set; }
        public bool RequieresOperationTeamSsoma { get; set; }
        public int CurrentStatusId { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? SubmissionsDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? GeneralObservation { get; set; }
    }
}
