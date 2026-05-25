using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedKernel.Constants;

namespace Application.Common.Validations
{
    public static class FileValidationExtensions
    {
        public static IRuleBuilderOptions<T, IFormFile?> MustBeValidImage<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder)
        {
            return ruleBuilder.Must(file =>
            {
                if (file == null || file.Length == 0) return true; 

                using var stream = file.OpenReadStream();
                var buffer = new byte[8];
                stream.Read(buffer, 0, buffer.Length);

                // JPG: FF D8 FF
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF) return true;
                // PNG: 89 50 4E 47
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47) return true;
                // WebP: RIFF .... WEBP
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46) return true;

                return false;
            }).WithMessage("El archivo '{PropertyName}' no es una imagen permitida (JPG, PNG, WEBP).");
        }

        public static IRuleBuilderOptions<T, IFormFile?> MaxSizeInBytes<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, long maxSizeBytes)
        {
            return ruleBuilder.Must(file =>
            {
                if (file == null) return true;
                return file.Length <= maxSizeBytes;
            }).WithMessage($"El archivo '{{PropertyName}}' excede el tamaño máximo permitido de {maxSizeBytes / FileSize.MB}MB.");
        }
    }
}
