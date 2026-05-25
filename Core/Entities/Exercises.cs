using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Exercises
    {
        public long ExercisesId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "1";
        public bool IndBlock { get; set; } = true;
        public long UsersBy { get; set; }
        public int ExercisesCount { get; set; }
    }
}
