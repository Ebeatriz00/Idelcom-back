using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Audit
{
    public class AuditLogFactory(IHttpContextAccessor httpContextAccessor) : IAuditLogFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Crea una instancia de <see cref="AuditLog"/> con la información base de auditoría
        /// asociada al registro afectado y al contexto de la solicitud actual.
        /// </summary>
        /// <param name="businessId">Identificador de la empresa asociada al registro auditado.</param>
        /// <param name="tableName">Nombre de la tabla sobre la cual se realiza la operación.</param>
        /// <param name="recordId">Identificador del registro afectado.</param>
        /// <param name="userId">Identificador del usuario que ejecuta la operación.</param>
        /// <param name="comment">Comentario adicional de auditoría. Puede ser nulo.</param>
        /// <returns>
        /// Una instancia de <see cref="AuditLog"/> con los datos principales de auditoría,
        /// incluyendo módulo e IP del cliente si pueden resolverse desde el contexto HTTP.
        /// </returns>
        public AuditLog Create(long businessId, string tableName, long recordId, long userId, string? comment = null)
        {
            var context = _httpContextAccessor.HttpContext;

            return new AuditLog
            {
                BusinessId = businessId,
                TableName = tableName,
                RecordId = recordId,
                CreateUser = userId,
                Comment = comment,
                ModuleName = ResolveModuleName(context),
                IpAddress = ResolveClientIp(context)
            };
        }

        /// <summary>
        /// Resuelve el nombre del módulo asociado a la solicitud HTTP actual.
        /// </summary>
        /// <param name="context">
        /// Contexto HTTP de la solicitud actual. Puede ser nulo.
        /// </param>
        /// <returns>
        /// El nombre del controlador si está disponible en los valores de ruta;
        /// en caso contrario, intenta obtenerlo a partir de los segmentos de la URL.
        /// Retorna <c>null</c> si no es posible determinarlo.
        /// </returns>
        private static string? ResolveModuleName(HttpContext? context)
        {
            if (context == null)
                return null;

            if (context.Request.RouteValues.TryGetValue("controller", out var controller) && controller != null)
                return controller.ToString();

            var path = context.Request.Path.Value;
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length >= 2 ? segments[1] : segments.FirstOrDefault();
        }

        /// <summary>
        /// Resuelve la dirección IP del cliente a partir del contexto HTTP actual.
        /// </summary>
        /// <param name="context">
        /// Contexto HTTP de la solicitud actual. Puede ser nulo.
        /// </param>
        /// <returns>
        /// La IP del cliente obtenida desde los encabezados de proxy
        /// (<c>CF-Connecting-IP</c>, <c>X-Real-IP</c>, <c>X-Forwarded-For</c>)
        /// o, en su defecto, desde la conexión remota.
        /// Devuelve <c>null</c> si no es posible determinarla.
        /// </returns>
        private static string? ResolveClientIp(HttpContext? context)
        {
            if (context == null)
                return null;

            var cf = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(cf))
                return cf;

            var xri = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xri))
                return xri;

            var xff = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xff))
            {
                var first = xff.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(first))
                    return first;
            }

            var ip = context.Connection.RemoteIpAddress;
            if (ip == null)
                return null;

            if (System.Net.IPAddress.IsLoopback(ip))
                return "127.0.0.1";

            return ip.MapToIPv4().ToString();
        }
    }
}
