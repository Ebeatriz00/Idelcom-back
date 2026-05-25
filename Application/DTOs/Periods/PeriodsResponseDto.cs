using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periods
{
    public class PeriodsResponseDto
    {
        public long PeriodsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "1";
        public bool IndBlock { get; set; }
        public string ExerciseDescription { get; set; } = string.Empty;
        public DateTime ExercisesEndDate { get; set; } 
        public int PeriodsCount { get; set; }
    }
}
