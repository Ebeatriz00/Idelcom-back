using Core.Interfaces;
using Core.Interfaces.Abstractions;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.Auth.AuthDtos;

namespace Application.UseCases.Auth
{
    public sealed class VerifyOtp
    {
        private readonly IUsersRepository _users;
        private readonly IOtpRepository _otps;
        private readonly ITenantProvider _tenant;
        private readonly IDateTime _clock;
        private readonly IPasswordResetRepository _pass;

        // Deja un solo constructor y asigna todos los readonly:
        public VerifyOtp(
            IUsersRepository users,
            IOtpRepository otps,
            ITenantProvider tenant,
            IDateTime clock,
            IPasswordResetRepository pass)
        {
            _users = users;
            _otps = otps;
            _tenant = tenant;
            _clock = clock;
            _pass = pass;
        }

        public async Task<string> Handle(VerifyOtpCommand cmd)
        {
            var tenantId = _tenant.TenantId;

            // Evita 'new("mensaje")'; especifica el tipo de excepción:
            //var user = await _users.GetByEmailAsync(tenantId, cmd.Email)
            //           ?? throw new InvalidOperationException("Usuario no encontrado");

            //var otp = await _otps.GetActiveAsync(tenantId, user.Id)
            //          ?? throw new InvalidOperationException("OTP no encontrado");

            //if (otp.ExpiresAtUtc < _clock.UtcNow)
            //    throw new InvalidOperationException("OTP expirado");

            //if (otp.Code != cmd.Code)
            //{
            //    await _otps.IncrementAttemptsAsync(otp.Id);
            //    throw new InvalidOperationException("Código incorrecto");
            //}

            //await _otps.ConsumeAsync(otp.Id);

            var token = Guid.NewGuid().ToString("N");
            var expires = _clock.UtcNow.AddMinutes(15);
            //await _pass.CreateAsync(tenantId, user.Id, token, expires);

            return token;
        }
    }

}
