namespace Application.DTOs.AppAuth
{
    public class AppLoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// El identificador único del dispositivo móvil desde el cual se realiza el login (ej. UUID).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre descriptivo del dispositivo (ej. "Samsung Galaxy S23")
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// Marca del dispositivo
        /// </summary>
        public string DeviceBrand { get; set; } = string.Empty;

        /// <summary>
        /// Versión del dispositivo.
        /// </summary>
        public string DeviceVersion { get; set; } = string.Empty;

        /// <summary>
        /// Version del aplicativo.
        /// </summary>
        public string AppVersion { get; set; } = string.Empty;
    }
}
