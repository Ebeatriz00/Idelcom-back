using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ILinkTokenService
    {
        string Issue(string entity, long resourceId, TimeSpan ttl);
        bool TryValidate(string token, out string entity, out long resourceId);
        bool ValidateToken(string token,out ClaimsPrincipal? principal, out string entity, out string? resourceId);
    }
}
