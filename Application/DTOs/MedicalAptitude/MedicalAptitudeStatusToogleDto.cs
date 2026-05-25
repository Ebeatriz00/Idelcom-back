using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MedicalAptitude
{
    public class MedicalAptitudeStatusToogleDto
    {
        public long MedicalAptitudeId { get; set; }
        public long BusinessId { get; set; }
        public string Status { get; set; }
        public long UsersBy { get; set; }
    }
}
