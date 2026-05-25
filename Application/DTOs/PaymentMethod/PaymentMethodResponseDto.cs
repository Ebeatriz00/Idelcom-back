using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentMethod
{
    public class PaymentMethodResponseDto
    {
        public long PaymentMethodId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Days { get; set; }
        public string Status { get; set; } = "1";
        public string PMConditionDescription { get; set; } = string.Empty;
        public string PMVisDescription { get; set; } = string.Empty;
        public int PaymentMethodCount { get; set; }
    }
}
