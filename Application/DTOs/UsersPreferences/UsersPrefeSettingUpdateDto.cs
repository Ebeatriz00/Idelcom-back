using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UsersPreferences
{
    public class UsersPrefeSettingUpdateDto
    {
        public long UsersId { get; set; }
        public long BusinessId { get; set; }
        public string Theme { get; set; }
        public string Density { get; set; }
    }
}
