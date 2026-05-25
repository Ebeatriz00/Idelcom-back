using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProcessType
{
    public class ProcessTypeCreateDto
    {
        public long BusinessId { get; set; }
        public string DescType { get; set; }
        public long UsersBy { get; set; }
    }
}
