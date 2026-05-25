using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PaymentMethod
    {
        public long PaymentMethodId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Days { get; set; }
        public long PMConditionId { get; set; }
        public long PMVisId { get; set; }
        public long UsersBy {  get; set; }
        public string Status { get; set; } = "1";
        public string PMConditionDescription { get; set; }
        public string PMVisDescription { get; set; }
        public int PaymentMethodCount { get; set; }
    }
}
