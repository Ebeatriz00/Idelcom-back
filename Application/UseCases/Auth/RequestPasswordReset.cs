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
    public sealed class RequestPasswordReset
    {
        private readonly IUsersRepository _users; private readonly IOtpRepository _otps; private readonly IEmailSender _email; private readonly ITenantProvider _tenant; private readonly IDateTime _clock;
        public RequestPasswordReset(IUsersRepository users, IOtpRepository otps, IEmailSender email, ITenantProvider tenant, IDateTime clock) { _users = users; _otps = otps; _email = email; _tenant = tenant; _clock = clock; }


        public async Task<OkResponse> Handle(RequestResetCommand cmd)
        {
            var tenantId = cmd.Tenant ?? _tenant.TenantId; // fallback
            //var user = await _users.GetByEmailAsync(tenantId, cmd.Email);
            //if (user is null || !user.IsActive) return new OkResponse(); // no revelar


            // genera OTP 6 dígitos + cooldown 60s
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var expires = _clock.UtcNow.AddMinutes(10);
           //wait _otps.UpsertAsync(tenantId, user.Id, code, expires);


            var html = $"Tu código de verificación es <b>{code}</b>. Expira en 10 minutos.";
           //wait _email.SendAsync(user.Email, "Código de verificación", html);
            return new OkResponse(true, 60);
        }
    }
}
