using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicalAptitude
{
    public class MedicalAptitudeSelectDto
    {
        public long MedicalAptitudeId { get; set; }
        public string MedicalAptitudeDesc { get; set; } = string.Empty;
    }
}
