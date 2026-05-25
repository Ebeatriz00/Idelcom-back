using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentType
{
    public class PaymentTypeUpdateDto
    {
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int UsersBy { get; set; }
    }
}
