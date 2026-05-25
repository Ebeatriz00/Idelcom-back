using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users
{
    public class UsersUpdateDto
    {

        public long BusinessId { get; set; }
        public long? WorkerId { get; set; }
        public long UsersId { get; set; }
        public string UsersName { get; set; } = string.Empty;
        public string UsersLastName { get; set; } = string.Empty;
        public string UsersCode { get; set; } = string.Empty;
        public string UsersEmail { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
        public int ProfilesId { get; set; }
        public string UsersDocument { get; set; } = string.Empty;
        public string UsersPhoto { get; set; } = string.Empty;
        public int UsersBy { get; set; }
    }
}
