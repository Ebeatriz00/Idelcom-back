using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task RevokeSessionAsync(string sid, TimeSpan ttl, CancellationToken ct = default);
        Task<bool> IsRevokedAsync(string sid, CancellationToken ct = default);
    }
}
