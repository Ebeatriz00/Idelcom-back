namespace Core.Entities.Security
{
    public class AppDeviceSession
    {
        /// <summary>
        /// Identificador interno del registro
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ID del usuario logueado en la App
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Identificador único del dispositivo móvil (ej. UUID)
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del dispositivo.
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// Marca del dispositivo
        /// </summary>
        public string DeviceBrand { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de versión del dispositivo.
        /// </summary>
        public string DeviceVersion { get; set; } = string.Empty;

        /// <summary>
        /// Versión de la aplicación.
        /// </summary>
        public string AppVersion { get; set; } = string.Empty;

        /// <summary>
        /// JWT ID único generado en el momento del login
        /// </summary>
        public string Jti { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del token / inicio de sesión
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indica si la sesión fue revocada (Blacklisted)
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Fecha en que se revocó la sesión (si aplica)
        /// </summary>
        public DateTime? RevokedAt { get; set; }
    }
}
