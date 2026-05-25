using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CommercialParameters
{
    public class CreateCommercialParametersDto
    {
        public long BusinessId { get; set; }
        public string? ParametersName { get; set; }
        public int ParametersValue { get; set; }
        public int? MinValue { get; set; }
        public long UsersBy { get; set; }
    }
}
