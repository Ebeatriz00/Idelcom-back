using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicalAptitude
{
    public class MedicalAptitudeResponseDto
    {
        public long MedicalAptitudeId { get; set; }
        public long BusinessId { get; set; }
        public string MedicalAptitudeDesc { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
