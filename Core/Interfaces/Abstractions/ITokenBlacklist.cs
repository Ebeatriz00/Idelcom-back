using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Abstractions
{
    public interface ITokenBlacklist
    {
        Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default);
        Task BlacklistAsync(string jti, TimeSpan ttl, CancellationToken ct = default);
    }
}
