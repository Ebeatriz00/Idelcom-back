using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Profiles
{
    public class ProfilesResponseDto
    {
        public long ProfilesId { get; set; }
        public long BusinessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long UsersCount { get; set; }
        public string Status { get; set; } = "1";
    }
}
