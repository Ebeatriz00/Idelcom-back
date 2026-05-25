using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Paginations
{
    public class PagedResultDtos<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize); 
    }
}
