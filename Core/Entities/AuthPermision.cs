using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AuthAllowedModules
    {
        public long? ModulesId { get; set; }
        public string? Code { get; set; }
        public string? Label { get; set; }
        public string? Path { get; set; }
        public string? IconKey { get; set; }
        public long? ParentModulesId { get; set; }
        public long? ParentId { get; set; }
        public int? OrderNo { get; set; }
    }
    public class AuthEffectivePerms
    {
        public long ModulesId { get; set; }
        public string ModulesCode { get; set; } = "";
        public long PermissionsId { get; set; }
        public string PermissionsName { get; set; } = "";
    }
    public sealed class AuthCacheKeyInfo
    {
        public long ProfilesId { get; init; }
    }
}
