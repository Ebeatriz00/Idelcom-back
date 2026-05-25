using Application.Abstractions;
using Application.DTOs.Auth;
using Application.Services.InterfacesServices;
using static Application.Abstractions.IUseCase;

namespace Application.UseCases.Auth
{
    public sealed record GetAuthBootstrapRequest(long usersId, long businessId);

    public sealed class GetAuthBootstrapUseCase
        : IUseCase<GetAuthBootstrapRequest, AuthBootstrapDto>
    {
        private readonly IAuthPermissionService _service;

        public GetAuthBootstrapUseCase(IAuthPermissionService service)
            => _service = service;

        public async Task<AuthBootstrapDto> Handle(GetAuthBootstrapRequest request, CancellationToken ct)
        {
            if (request.usersId <= 0 || request.businessId <= 0)
                throw new ArgumentException("Parámetros inválidos para bootstrap de auth.");

            var permsTask = _service.GetEffectiveSetAsync(request.usersId, request.businessId, ct);  // AuthEffectivePermsDto[]
            var modsTask = _service.GetAllowedModulesAsync(request.usersId, request.businessId, ct); // AuthAllowedModulesDto[]

            await Task.WhenAll(permsTask, modsTask);

            var perms = permsTask.Result ?? Array.Empty<AuthEffectivePermsDto>();
            var mods = modsTask.Result ?? Array.Empty<AuthAllowedModulesDto>();

            var flat = perms
                .Where(p => !string.IsNullOrWhiteSpace(p.ModulesCode) && !string.IsNullOrWhiteSpace(p.PermissionsName))
                .Select(p => $"{p.ModulesCode}:{p.PermissionsName}".ToLowerInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var allowedCodes = mods
                .Select(m => m.Code)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var hash = _service.ComputeHash(flat.Concat(allowedCodes));

            return new AuthBootstrapDto
            {
                AllowedModuleCodes = allowedCodes,  
                AllowedModules = mods,             
                EffectiveList = flat,
                Hash = hash
            };
        }
    }
}
