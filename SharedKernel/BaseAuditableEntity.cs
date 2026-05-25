using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class BaseAuditableEntity
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public long CreateUser { get;  set; }
        public DateTime? UpdateDate { get;  set; }
        public long? UpdateUser { get;  set; }
        public string? Status { get;  set; }
    }
}
