using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Logistic
{
    public class ProductTypeItem
    {
        public long ProductTypesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = null!;
        public string IsConsumable { get; set; } = null!;
        public string IsReturnable { get; set; } = null!;
        public string RequiresSerial { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
