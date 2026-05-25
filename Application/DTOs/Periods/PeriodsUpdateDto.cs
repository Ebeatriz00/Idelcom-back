using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periods
{
    public class PeriodsUpdateDto
    {
        public long PeriodsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long ExercisesId { get; set; }
        public DateTime EndDate { get; set; }
        public long UsersBy { get; set; }
    }
}
