using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Operations.Operations
{
    public class OperationsCreateDto
    {
        public long BusinessId { get; set; }
        public long OpporId { get; set; }
        public long? QualitySupervisorId { get; set; }
        public long? ProjectManagerId { get; set; }
        public bool? RequeredSsoma { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? OperationsStatusId { get; set; }
    }
}
