using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PMCondition
{
    public class PMConditionStatusToggleDto
    {
        public long PMConditionId { get; set; }
        public long BussinessId { get; set; }
        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
    }
}
