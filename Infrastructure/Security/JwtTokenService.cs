using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Interfaces.Services;
using Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public sealed class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _opt;
        private readonly SigningCredentials _creds;

        public JwtTokenService(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;

            if (string.IsNullOrWhiteSpace(_opt.Key) || _opt.Key.Length < 32)
                throw new ArgumentException("JwtOptions.Key debe tener al menos 32 caracteres.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public string Create(long usersId, long businessId, string? profileName, string sid)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usersId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),

                new(ClaimTypes.NameIdentifier, usersId.ToString()),
                new("uid", usersId.ToString()),
                new("bid", businessId.ToString()),
                new("sid", sid)
            };

            if (!string.IsNullOrWhiteSpace(profileName))
            {
                claims.Add(new(ClaimTypes.Role, profileName));
                claims.Add(new("profile", profileName));
            }

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_opt.ExpireMinutes),
                signingCredentials: _creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}