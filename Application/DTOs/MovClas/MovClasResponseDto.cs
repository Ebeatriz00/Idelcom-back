using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovClas
{
    public class MovClasResponseDto
    {
        public long MovClasId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public int MovClasCount { get; set; }
    }
}
