using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CommercialParameters
    {
        public int CommercialParametersId { get; set; }
        public long BusinessId { get; set; }
        public string? ParametersName { get; set; }
        public int ParametersValue { get; set; }
        public int? MinValue { get; set; }
        public long UsersBy { get; set; }
        public string? Status { get; set; }
    }
}
