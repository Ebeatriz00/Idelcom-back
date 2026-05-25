using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentType
{
    public class PaymentTypeStatusToggleDto
    {
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public int UsersBy { get; set; }
        public string Status { get; set; }
    }
}
