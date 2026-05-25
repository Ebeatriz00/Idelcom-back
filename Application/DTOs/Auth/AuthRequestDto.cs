using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public sealed class AuthRequestDto
    {
        public string UsersKey { get; init; } = string.Empty;
        public string UsersPassword { get; set; } = string.Empty;
    }
}
