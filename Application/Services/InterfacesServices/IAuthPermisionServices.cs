using Application.DTOs.Auth;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.InterfacesServices
{
    public interface IAuthPermissionService
    {
        Task<AuthEffectivePermsDto[]> GetEffectiveSetAsync(long usersId, long businessId, CancellationToken ct = default);
        Task<AuthAllowedModulesDto[]> GetAllowedModulesAsync(long usersId, long businessId, CancellationToken ct = default);
        string ComputeHash(IEnumerable<string> items);
        void Invalidate(long profilesId, long businessId);
    }
}
