using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AuditFieldAttribute(string alias) : Attribute
    {
        public string Alias { get; set; } = alias;
    }
}
