using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UsersPreferences
{
    public class UsersPrefeNotiUpdateDto
    {
        public long UsersId { get; set; }
        public long BusinessId { get; set; }
        public bool EmailNotif { get; set; }
        public bool PushNotif { get; set; }
    }
}
