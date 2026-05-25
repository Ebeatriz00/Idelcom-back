using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periods
{
    public class PeriodsCreateDto
    {
        public long BusinessId { get; set; }
        public long ExercisesId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public long UsersBy { get; set; }
    }
}
