using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ILoginAttemptService
    {
        Task<bool> IsLockedOutAsync(string key, CancellationToken ct = default);
        Task RegisterFailureAsync(string key, CancellationToken ct = default);
        Task ResetAsync(string key, CancellationToken ct = default);
    }
}
