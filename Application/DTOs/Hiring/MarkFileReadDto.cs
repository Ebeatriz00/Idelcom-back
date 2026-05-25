using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Hiring
{
    public class MarkFileReadDto
    {
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
        public string OpporToken { get; set; } = string.Empty; 
    }
}
