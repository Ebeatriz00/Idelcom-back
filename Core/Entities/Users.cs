using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Users
    {
        public long BusinessId { get; set; }
        public long? WorkerId { get; set; }
        public long UsersId { get; set; }
        public string UsersName { get; set; } = string.Empty;
        public string UsersLastName { get; set; } = string.Empty;
        public string UsersCode { get; set; } = string.Empty;
        
        public string UsersEmail { get; set; } = string.Empty;
        public long DocumentTypeId { get; set; }
        public long ProfilesId { get; set; }
        public string UsersDocument { get; set; } = string.Empty;
        public string UsersPhoto { get; set; } = string.Empty;
        public string UsersPassword { get; set; } = string.Empty;
        public string UsersSalt { get; set; } = string.Empty;


        public string Status { get; set; } = "1";
        public int UsersBy { get; set; }

        public string User { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DescriptionProfiles { get; set; } = string.Empty;


        public Guid Id { get; init; }
        public string Email { get; init; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsActive { get; set; }
        public string TenantId { get; init; } = default!;

    }
}
