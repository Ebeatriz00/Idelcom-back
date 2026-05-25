using Core.Interfaces;
using Core.Interfaces.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.Auth.AuthDtos;

namespace Application.UseCases.Auth
{
    public sealed class ResetPassword
    {
        private readonly IPasswordResetRepository _pass; private readonly IUsersRepository _users; private readonly IPasswordHasher _hasher; private readonly ITenantProvider _tenant; private readonly IDateTime _clock;
        public ResetPassword(IPasswordResetRepository pass, IUsersRepository users, IPasswordHasher hasher, ITenantProvider tenant, IDateTime clock) { _pass = pass; _users = users; _hasher = hasher; _tenant = tenant; _clock = clock; }


        public async Task<OkResponse> Handle(ResetPasswordCommand cmd)
        {
            var tenantId = cmd.Tenant ?? _tenant.TenantId;
            var pr = await _pass.GetByTokenAsync(tenantId, cmd.ResetToken) ?? throw new("Token inválido");
            if (pr.ExpiresAtUtc < _clock.UtcNow || pr.ConsumedAtUtc is not null) throw new("Token expirado/consumido");
            // obten usuario por Id (agrega método en repo)
            var user = await _users.GetByIdAsync(pr.UserId) ?? throw new("Usuario no existe");
            user.PasswordHash = _hasher.Hash(cmd.NewPassword);
           //wait _users.UpdatePasswordAsync(user.TenantId, user.PasswordHash);
            await _pass.ConsumeAsync(pr.Id);
            return new OkResponse(true);
        }
    }
}
