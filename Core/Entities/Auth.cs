using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Auth
    {
        public long UsersId { get; set; }
        public long? WorkerId { get; set; }
        public long BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string UsersName { get; set; }
        public string UsersLastName { get; set; }
        public string ProfilesName { get; set; }
        public long AreasId { get; set; }
        public long UsersVisibiliyId { get; set; }
        public string UsersKey { get; set; }
        public string UsersPassword { get; set; }
        public string UsersSalt { get; set; }
        public string ApiToken { get; set; } = string.Empty;
        public string CodeLicense { get; set; }
        public string BusinessRuc { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }

        public string? FullName { get; set; }
        public string? UsersEmail { get; set; }
        public string? UserPhoto { get; set; }
        public long ProfilesId { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }

 
}
