using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentMethod
{
    public class PaymentMethodByIdDto
    {
        public long PaymentMethodId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Days { get; set; } = string.Empty;
        public long PMConditionId { get; set; }
        public long PMVisId { get; set; }
    }
}
