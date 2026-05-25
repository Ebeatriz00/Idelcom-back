using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Files
{
    public class FileUrlBuilder(IHttpContextAccessor httpContextAccessor) : IFileUrlBuilder
    {
        private const string FilesRoute = "/api/files";
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string BuildFileUrl(Guid fileUid)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : string.Empty;

            return $"{baseUrl}{FilesRoute}/{fileUid}";
        }

        public string BuildFileUrl(Guid? fileUid)
        {
            return fileUid.HasValue ? BuildFileUrl(fileUid.Value) : string.Empty;
        }
    }
}
