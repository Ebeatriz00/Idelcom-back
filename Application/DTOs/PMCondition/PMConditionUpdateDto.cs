using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PMCondition
{
    public class PMConditionUpdateDto
    {
        public long PMConditionId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public long UsersBy { get; set; }
    }
}
