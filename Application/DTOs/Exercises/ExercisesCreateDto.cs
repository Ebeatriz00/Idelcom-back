using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Exercises
{
    public class ExercisesCreateDto
    {
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public long UsersBy { get; set; }

    }
}
