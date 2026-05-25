using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Operations.WorkDayStatus
{
    public class WorkDayStatusGetByIdDto
    {
        public int WorkDayStatusId { get; set; }
        public long BusinessId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
