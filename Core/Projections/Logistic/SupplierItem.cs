using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Logistic
{
    public class SupplierItem
    {
        public long SuppliersId { get; set; }
        public long BusinessId { get; set; }
        public string? SuppliersTypeDesc { get; set; }
        public string? SuppliersGroupDesc { get; set; }
        public string? SupplierName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ContactName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? PaymentMethodDesc { get; set; }
        public string? PaymentConditionDesc { get; set; }
        public string? Status { get; set; }

    }
}
