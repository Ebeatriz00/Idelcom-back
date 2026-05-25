using Application.DTOs.Auth;
using Application.Services.InterfacesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Abstractions.IUseCase;

namespace Application.UseCases.Auth
{
    public class GetEffectivePermissions
    {
        public sealed record GetEffectivePermissionsRequest(long usersId, long businessId);

        public sealed class GetEffectivePermissionsUseCase
            : IUseCase<GetEffectivePermissionsRequest, AuthEffectivePermsDto[]>
        {
            private readonly IAuthPermissionService _service;

            public GetEffectivePermissionsUseCase(IAuthPermissionService service)
                => _service = service;

            public Task<AuthEffectivePermsDto[]> Handle(GetEffectivePermissionsRequest request, CancellationToken ct)
            {
                if (request.usersId <= 0 || request.businessId <= 0)
                    throw new ArgumentException("Parámetros inválidos para permisos efectivos.");

                return _service.GetEffectiveSetAsync(request.usersId, request.businessId, ct);
            }
        }
    }
}
