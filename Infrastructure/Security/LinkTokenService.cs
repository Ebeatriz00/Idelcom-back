using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class LinkTokenService : ILinkTokenService
    {
        private readonly string _secret;
        public LinkTokenService(IConfiguration cfg) => _secret = cfg["JwtSetting:Key"]!;

        public string Issue(string entity, long resourceId, TimeSpan ttl)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                claims: new[] { new Claim("ent", entity), new Claim("rid", resourceId.ToString()) },
                notBefore: now,
                expires: now.Add(ttl),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public bool TryValidate(string token, out string entity, out long resourceId)
        {
            entity = default!; resourceId = default;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                }, out var validated);

                var jwt = (JwtSecurityToken)validated;
                entity = jwt.Claims.First(c => c.Type == "ent").Value;
                resourceId = long.Parse(jwt.Claims.First(c => c.Type == "rid").Value);
                return true;
            }
            catch { return false; }
        }

        public bool ValidateToken(
        string token,
        out ClaimsPrincipal? principal,
        out string entity,
        out string? resourceId)
        {
            principal = null;
            entity = string.Empty;
            resourceId = null;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                    ValidateIssuer = false,  
                    ValidateAudience = false,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromHours(1) 
                };

                principal = handler.ValidateToken(token, parameters, out var validated);
                if (validated is not JwtSecurityToken jwt)
                    return false;

                // entity (claim "ent")
                entity = jwt.Claims.FirstOrDefault(c => c.Type == "ent")?.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(entity))
                    return false;

                // resourceId opcional (claim "rid")
                resourceId = jwt.Claims.FirstOrDefault(c => c.Type == "rid")?.Value;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
