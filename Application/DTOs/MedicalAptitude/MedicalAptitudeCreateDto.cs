using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicalAptitude
{
    public class MedicalAptitudeCreateDto
    {
        public long BusinessId { get; set; }
        public string MedicalAptitudeDesc { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
