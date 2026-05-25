using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UsersPreferences
    {
        public long UsersId { get; set; }
        public long BusinessId { get; set; }
        public bool EmailNotif { get; set; }
        public bool PushNotif { get; set; }
        public string Language { get; set; }
        public string Timezone { get; set; }
        public string Theme { get; set; }
        public string Density { get; set; }
    }
}
