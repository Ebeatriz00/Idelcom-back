using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Sube un archivo al almacenamiento físico y lo registra en la base de datos.
        /// </summary>
        /// <param name="stream">Flujo de datos del archivo.</param>
        /// <param name="originalFileName">Nombre original del archivo con extensión.</param>
        /// <param name="suggestedPath">Ruta sugerida (ej. "Modulo/Submodulo").</param>
        /// <param name="userId">ID del usuario que realiza la carga.</param>
        /// <returns>GUID del archivo registrado.</returns>
        Task<Guid> UploadAsync(Stream stream, string originalFileName, string suggestedPath, long userId);

        /// <summary>
        /// Recupera el flujo de datos y metadatos de un archivo mediante su GUID.
        /// </summary>
        /// <param name="fileId">Identificador único del archivo.</param>
        /// <returns>Objeto con el stream y metadatos del archivo.</returns>
        Task<FileDownloadResult> GetStreamAsync(Guid fileId);

        /// <summary>
        /// Elimina un archivo físicamente y su registro en la base de datos de forma coordinada.
        /// </summary>
        /// <param name="fileId">Identificador único del archivo.</param>
        Task<bool> CleanupAsync(Guid fileId);
    }

    public record FileDownloadResult(Stream Stream, string MimeType, string FileName);
}
