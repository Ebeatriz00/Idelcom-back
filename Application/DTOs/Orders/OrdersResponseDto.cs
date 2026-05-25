using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Orders
{
    public class OrdersResponseDto
    {
        public long OperationsId { get; set; }
        public string OpporToken { get; set; }
        public long BusinessId { get; set; }
        public long OpporId { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public string? ClientsName { get; set; }
        public string? Commercial { get; set; }
        public string? Responsible { get; set; }
        public string? QualitySupervisor { get; set; }
        public string? ProjectManager { get; set; }
        public string? Ssoma { get; set; }
        public string? Status { get; set; }
        public long UsersBy { get; set; }
        public string? SsomaIds { get; set; }
        public Boolean? RequeredSsoma { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public string? TypeOppor { get; set; }
        public long? ParentOpportunityId { get; set; }
        public int? AdditionalSequence { get; set; }
        public string? StateColor { get; set; }
    }
}
