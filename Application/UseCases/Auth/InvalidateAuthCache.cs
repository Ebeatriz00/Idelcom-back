using Application.Services.InterfacesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Abstractions.IUseCase;

namespace Application.UseCases.Auth
{
    public sealed record InvalidateAuthCacheCommand(long profilesId, long businessId);

    public sealed class InvalidateAuthCacheUseCase
        : IUseCase<InvalidateAuthCacheCommand, bool>
    {
        private readonly IAuthPermissionService _service;

        public InvalidateAuthCacheUseCase(IAuthPermissionService service)
            => _service = service;

        public Task<bool> Handle(InvalidateAuthCacheCommand request, CancellationToken ct)
        {
            if (request.profilesId <= 0 || request.businessId <= 0)
                throw new ArgumentException("Parámetros inválidos para invalidar caché.");

            _service.Invalidate(request.profilesId, request.businessId);
            return Task.FromResult(true);
        }
    }
}
