using Core.Entities.Security;

namespace Core.Interfaces.Security
{
    public interface IAppDeviceSessionRepository
    {
        /// <summary>
        /// Registra una nueva sesión de dispositivo al iniciar sesión.
        /// </summary>
        Task<long> CreateAsync(AppDeviceSession session);

        /// <summary>
        /// Verifica si un token (JTI) ha sido revocado en la base de datos.
        /// </summary>
        Task<bool> IsRevokedAsync(string jti);

        /// <summary>
        /// Revoca una sesión específica por su JTI.
        /// </summary>
        Task<bool> RevokeByJtiAsync(string jti);

        /// <summary>
        /// Revoca todas las sesiones activas de un dispositivo específico.
        /// </summary>
        Task<bool> RevokeByDeviceIdAsync(string deviceId);
    }
}
