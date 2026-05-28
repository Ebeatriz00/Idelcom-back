using Core.Entities;
using Core.Interfaces;
using Core.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices
{
    public class FileStorageService(IFileRepository fileRepository, IOptions<StorageOptions> options) : IStorageService
    {
        private readonly IFileRepository _fileRepository = fileRepository;
        private readonly StorageOptions _options = options.Value;

        public async Task<Guid> UploadAsync(Stream stream, string originalFileName, string suggestedPath, long userId)
        {
            // 1. Sanitización y preparación de rutas
            string sanitizedPath = SanitizePath(suggestedPath);
            string yearMonth = DateTime.Now.ToString("yyyy/MM");
            string relativeDirectory = Path.Combine(yearMonth, sanitizedPath).Replace("\\", "/");
            string absoluteDirectory = Path.Combine(_options.RootPath, relativeDirectory);

            if (!Directory.Exists(absoluteDirectory))
                Directory.CreateDirectory(absoluteDirectory);

            // 3. Renombrado físico
            Guid fileId = Guid.NewGuid();
            string extension = Path.GetExtension(originalFileName);
            string physicalName = $"{fileId}{extension}";
            string physicalPath = Path.Combine(absoluteDirectory, physicalName);
            string relativePath = Path.Combine(relativeDirectory, physicalName).Replace("\\", "/");

            // 4. Persistencia Atómica: Guardar en disco
            using (var fileStream = new FileStream(physicalPath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            try
            {
                // 5. Registro en Base de Datos
                var sysFile = new SysFile
                {
                    Id = fileId,
                    RelativePath = relativePath,
                    OriginalName = originalFileName,
                    FileSizeBytes = stream.Length,
                    MimeType = GetMimeType(extension),
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.Now
                };

                var response = await _fileRepository.RegisterFileAsync(sysFile);

                if (response.Status != 1)
                {
                    // Rollback físico si falla el registro en BD
                    if (File.Exists(physicalPath)) File.Delete(physicalPath);
                    throw new Exception($"Error al registrar archivo en BD: {response.Message}");
                }

                return fileId;
            }
            catch (Exception)
            {
                // Asegurar limpieza física en cualquier error post-guardado
                if (File.Exists(physicalPath)) File.Delete(physicalPath);
                throw;
            }
        }

        public async Task<FileDownloadResult> GetStreamAsync(Guid fileId)
        {
            var sysFile = await _fileRepository.GetFileByIdAsync(fileId);
            if (sysFile == null)
                throw new FileNotFoundException("El registro del archivo no existe en la base de datos.");

            string physicalPath = Path.Combine(_options.RootPath, sysFile.RelativePath);

            if (!File.Exists(physicalPath))
                throw new FileNotFoundException("El archivo físico no se encuentra en el servidor.");

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            return new FileDownloadResult(memoryStream, sysFile.MimeType, sysFile.OriginalName);
        }

        public async Task<bool> CleanupAsync(Guid fileId)
        {
            var sysFile = await _fileRepository.GetFileByIdAsync(fileId);
            if (sysFile == null) return false;

            string physicalPath = Path.Combine(_options.RootPath, sysFile.RelativePath);

            // Eliminar de la base de datos primero
            var response = await _fileRepository.DeleteFileAsync(fileId);

            if (response.Status == 1)
            {
                // Si se eliminó de BD, intentar eliminar del disco
                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);

                return true;
            }

            return false;
        }

        private static string SanitizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "default";

            // Eliminar intentos de path traversal y caracteres no válidos
            string sanitized = path.Replace("..", "")
                .Replace(":", "")
                .Replace("*", "")
                .Replace("?", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");
            return sanitized.TrimStart('/', '\\').TrimEnd('/', '\\');
        }

        private static string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream",
            };
        }
    }
}
