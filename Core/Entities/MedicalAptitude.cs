using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MedicalAptitude 
    {
        public long MedicalAptitudeId { get; set; }
        public long BusinessId { get; set; }
        public string MedicalAptitudeDesc { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}
