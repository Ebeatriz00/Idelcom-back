using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        // emite refresh para un usuario (ej. al loguear) y guarda server-side
        Task<string> IssueAsync(long userId, long businessId, string profilesName, string sid, string ip, string? device, TimeSpan lifetime, CancellationToken ct = default);

        // valida y rota: invalida el actual y devuelve (nuevoAccess, nuevoRefresh)
        Task<(string accessToken, string refreshToken)> RotateAsync(
            string refreshToken,
            Func<long, long, string ,string, string> issueAccessTokenForUser,
            string ip, string? device,
            TimeSpan newRefreshLifetime,
            CancellationToken ct = default);

        // revoca un refresh concreto (logout) y su familia
        Task RevokeAsync(string refreshToken, string reason, CancellationToken ct = default);
    }
}
