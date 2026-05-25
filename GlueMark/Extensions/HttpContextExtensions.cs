using System.Net;

namespace GlueMark.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Obtiene la IP del cliente respetando cabeceras comunes de proxy.
        /// </summary>
        public static string GetClientIp(this HttpContext ctx)
        {
            // 1) Cloudflare
            var cf = ctx.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(cf))
                return cf;

            // 2) X-Real-IP (Nginx)
            var xri = ctx.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xri))
                return xri;

            // 3) X-Forwarded-For (puede traer lista "ip1, ip2, ip3")
            var xff = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xff))
            {
                var first = xff.Split(',').Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(first))
                    return first;
            }

            // 4) Fallback al socket
            var ip = ctx.Connection.RemoteIpAddress;
            if (ip is null) return "unknown";

            // Normaliza ::1 → 127.0.0.1
            if (IPAddress.IsLoopback(ip)) return "127.0.0.1";
            return ip.MapToIPv4().ToString();
        }
    }
}
