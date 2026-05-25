using Core.Interfaces;
using Core.Interfaces.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.Auth.AuthDtos;

namespace Application.UseCases.Auth
{
    public sealed class ResendOtp
    {
        private readonly IUsersRepository _users; private readonly IOtpRepository _otps; private readonly IEmailSender _email; private readonly ITenantProvider _tenant; private readonly IDateTime _clock;
        public ResendOtp(IUsersRepository users, IOtpRepository otps, IEmailSender email, ITenantProvider tenant, IDateTime clock) { _users = users; _otps = otps; _email = email; _tenant = tenant; _clock = clock; }
        public async Task<OkResponse> Handle(ResendOtpCommand cmd)
        {
            var tenantId = _tenant.TenantId;
            //var user = await _users.GetByEmailAsync(tenantId, cmd.Email);
            //if (user is null || !user.IsActive) return new OkResponse();
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var expires = _clock.UtcNow.AddMinutes(10);
            //await _otps.UpsertAsync(tenantId, user.Id, code, expires);
            //await _email.SendAsync(user.Email, "Código de verificación", $"Tu código es <b>{code}</b>.");
            return new OkResponse(true, 60);
        }
    }
}
