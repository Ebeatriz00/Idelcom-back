using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovSunat
{
    public class MovSunatCreateDto
    {
        public long BusinessId { get; set; }
        public string Description { get; set; } 
        public long UsersBy { get; set; }
    }
}
