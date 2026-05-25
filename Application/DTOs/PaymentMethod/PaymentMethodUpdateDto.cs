using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentMethod
{
    public class PaymentMethodUpdateDto
    {
        public long PaymentMethodId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Days { get; set; }
        public long PMConditionId { get; set; }
        public long PMVisId { get; set; }
        public long UsersBy { get; set; }
    }
}
