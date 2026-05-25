using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UsersPreferences
{
    public class UsersPrefeUpdateDto
    {
        public long UsersId { get; set; }
        public long BusinessId { get; set; }
        public string Language { get; set; }
        public string Timezone { get; set; }
    }
}
