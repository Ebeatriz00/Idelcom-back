using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.paginations
{
    public sealed class PagedSelect<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public bool HasMore { get; set; } 
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
