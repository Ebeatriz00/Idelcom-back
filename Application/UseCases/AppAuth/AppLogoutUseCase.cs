using Application.Exceptions;
using Core.Interfaces.Security;
using SharedKernel;

namespace Application.UseCases.AppAuth
{
    public class AppLogoutUseCase(IAppDeviceSessionRepository appDeviceSessionRepository)
    {
        private readonly IAppDeviceSessionRepository _sessionRepository = appDeviceSessionRepository;

        public async Task<BaseResponse> ExecuteAsync(string jti, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(jti))
            {
                var errors = new List<GlobalErrorDetail> {
                    new ("VALIDATION", "El identificador del token (JTI) es requerido.")
                };

                throw new ValidationException(errors);
            }

            var revoked = await _sessionRepository.RevokeByJtiAsync(jti);

            return new BaseResponse
            {
                Status = revoked ? 1 : 0,
                Message = revoked ? "Sesión móvil cerrada exitosamente." : "La sesión ya estaba cerrada o no existe."
            };
        }
    }
}
