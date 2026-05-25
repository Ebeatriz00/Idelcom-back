using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovOper
{
    public class MovOperSelectDto
    {
        public long MovOperId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
