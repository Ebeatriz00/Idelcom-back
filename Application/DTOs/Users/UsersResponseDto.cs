using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users
{
    public class UsersResponseDto
    {
        public long BusinessId { get; set; }
        public long UsersId { get; set; }
        public string User { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string UsersDocument { get; set; } = string.Empty;
        public string DescriptionProfiles { get; set; } = string.Empty;
        public string UsersPhoto { get; set; } = string.Empty;
        public string Status { get; set; } = "1";

    }

    public class UsersResponseIdDto
    {
        public long BusinessId { get; set; }
        public long? WorkerId { get; set; }
        public long UsersId { get; set; }
        public string UsersName { get; set; } = string.Empty;
        public string UsersLastName { get; set; } = string.Empty;
        public string UsersCode { get; set; } = string.Empty;
        public string UsersEmail { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }

        public string UsersDocument { get; set; } = string.Empty;
        public int ProfilesId { get; set; }
        public string UsersPhoto { get; set; } = string.Empty;
        public string Status { get; set; } = "1";

    }
    public class UsersSettingResponseIdDto
    {
        public long UsersId { get; set; }
        public string UsersName { get; set; } = string.Empty;
        public string UsersLastName { get; set; } = string.Empty;
        public string UsersEmail { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DescriptionProfiles { get; set; } = string.Empty;
        public string UsersDocument { get; set; } = string.Empty;
        public string UsersPhoto { get; set; } = string.Empty;

    }
}
