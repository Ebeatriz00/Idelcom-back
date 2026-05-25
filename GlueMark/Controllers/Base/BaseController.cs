using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Idelcom.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected long GetRequiredLongClaim(string claimType)
        {
            var value = User.FindFirst(claimType)?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException($"No se encontró el claim {claimType}.");

            if (!long.TryParse(value, out var result))
                throw new UnauthorizedAccessException($"El claim {claimType} no es válido.");

            return result;
        }

        protected long GetCurrentUserId() => GetRequiredLongClaim("uid");
        protected long GetCurrentBusinessId() => GetRequiredLongClaim("bid");
    }
}
