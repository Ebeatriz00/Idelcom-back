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
    public sealed class BootstrapByToken
    {
        private readonly IPasswordResetRepository _pass; private readonly ITenantProvider _tenant; private readonly IDateTime _clock;
        public BootstrapByToken(IPasswordResetRepository pass, ITenantProvider tenant, IDateTime clock) { _pass = pass; _tenant = tenant; _clock = clock; }
        public async Task<BootstrapResponse> Handle(BootstrapByTokenQuery q)
        {
            var tenantId = q.Tenant ?? _tenant.TenantId;
            var pr = await _pass.GetByTokenAsync(tenantId, q.Token) ?? throw new("Token inválido");
            if (pr.ExpiresAtUtc < _clock.UtcNow || pr.ConsumedAtUtc is not null) throw new("Token expirado/consumido");
            return new(pr.ResetToken, null);
        }
    }
}
