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
    public class GetAllowedModules
    {
        public sealed record GetAllowedModulesRequest(long usersId, long businessId);

        public sealed class GetAllowedModulesUseCase
            : IUseCase<GetAllowedModulesRequest, AuthAllowedModulesDto[]>
        {
            private readonly IAuthPermissionService _service;

            public GetAllowedModulesUseCase(IAuthPermissionService service)
                => _service = service;

            public Task<AuthAllowedModulesDto[]> Handle(GetAllowedModulesRequest request, CancellationToken ct)
            {
                if (request.usersId <= 0 || request.businessId <= 0)
                    throw new ArgumentException("Parámetros inválidos para módulos permitidos.");

                return _service.GetAllowedModulesAsync(request.usersId, request.businessId, ct);
            }
        }
    }
}