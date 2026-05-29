using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsProjectConfing
{
   public class OperationsProjectConfigUpdateDto 
    {
        public long OperationsProjectConfigId { get; set; }
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public TimeSpan EntryTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public bool AllowDelay { get; set; }
        public int MinutesTolerance { get; set; }
        public TimeSpan BeforeOfficialTime { get; set; }
        public bool IsRequirePhoto { get; set; }
        public bool IsRequireOvertime { get; set; }
        public bool IsRequireOvertimeApproval { get; set; }
        public int Shift { get; set; }
        public bool? IsRequireAppAttendance { get; set; }
        public bool? IsRequireGroupPhoto { get; set; }
        public long? UpdateUser { get; set; }
    }
}
