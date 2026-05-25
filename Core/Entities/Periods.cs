using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Periods
    {
        public long PeriodsId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public long ExercisesId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public bool IndBlock { get; set; } = true;
        public int PeriodsCount { get; set; }
        public string ExercisesDescription { get; set; }
        public DateTime ExercisesEndDate { get; set; }

    }
}
