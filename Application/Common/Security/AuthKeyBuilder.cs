using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Security
{
    /// <summary>
    /// Construye la clave de intentos fallidos combinando UsersKey normalizado + IP.
    /// Guarda solo el hash del UsersKey para no exponer datos en cache/logs.
    /// </summary>
    public static class AuthKeyBuilder
    {
        public static string Build(string usersKeyNormalized, string ip)
        {
            // usersKeyNormalized: ya normalizado (email → lower, username → upper)
            using var sha = SHA256.Create();
            var keyBytes = Encoding.UTF8.GetBytes(usersKeyNormalized);
            var hashHex = Convert.ToHexString(sha.ComputeHash(keyBytes)); // ABCDEF…

            var safeIp = string.IsNullOrWhiteSpace(ip) ? "unknown" : ip;
            return $"{hashHex}|{safeIp}";
        }
    }
}
