using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentMethod
{
    public class PaymentMethodSelectDto
    {
        public long PaymentMethodId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}