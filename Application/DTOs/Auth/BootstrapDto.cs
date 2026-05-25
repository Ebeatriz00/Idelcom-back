using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public sealed class AuthBootstrapDto
    {
        public string[] AllowedModuleCodes { get; set; } = Array.Empty<string>();      
        public string[] EffectiveList { get; set; } = Array.Empty<string>();
        public AuthAllowedModulesDto[] AllowedModules { get; set; } = Array.Empty<AuthAllowedModulesDto>();
        public string Hash { get; set; } = "";
    }
}
