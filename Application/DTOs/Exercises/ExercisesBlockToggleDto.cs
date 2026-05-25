using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Exercises
{
    public class ExercisesBlockToggleDto
    {
        public long ExercisesId { get; set; }
        public long BusinessId { get; set; }
        public bool IndBlock { get; set; }
        public long UsersBy { get; set; }
    }
}
