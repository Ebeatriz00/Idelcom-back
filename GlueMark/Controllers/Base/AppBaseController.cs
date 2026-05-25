using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Idelcom.Controllers.Base
{
    [Authorize(Policy = "RequireAppAccess")]
    [ApiController]
    public abstract class AppBaseController : ControllerBase
    {
        protected long GetCurrentAppUserId()
        {
            // Buscamos "sub" o su equivalente mapeado por ASP.NET Core
            var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("No se encontró el identificador de usuario (sub) en el token.");

            if (!long.TryParse(value, out var result))
                throw new UnauthorizedAccessException("El identificador de usuario del token no es válido.");

            return result;
        }

        protected long GetCurrentAppBusinessId()
        {
            var value = User.FindFirst("bid")?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("No se encontró el identificador de negocio (bid) en el token.");

            if (!long.TryParse(value, out var result))
                throw new UnauthorizedAccessException("El identificador de negocio del token no es válido.");

            return result;
        }

        protected string GetCurrentAppJti()
        {
            // Buscamos "jti" o su equivalente mapeado
            var value = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                ?? User.FindFirst("jti")?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("No se encontró el identificador del token (jti).");

            return value;
        }

        protected string GetClientIp()
        {
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                     ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "unknown";

            return ip;
        }
    }
}
