using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PMCondition
{
    public class PMConditionCreateDto
    {
        public long BussinessId { get; set; }
        public string Description { get; set; }
        public long UsersBy { get; set; }
    }
}
