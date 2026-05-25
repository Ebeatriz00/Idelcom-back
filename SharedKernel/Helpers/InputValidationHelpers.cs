using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharedKernel.Helpers
{
    public static class InputValidationHelpers
    {
        private static readonly string[] _blacklist = new[]
        {
            "<SCRIPT", "</SCRIPT", "--", ";", "'", "\"", "/*", "*/", "DROP", "SELECT", "INSERT", "UPDATE", "DELETE",
            "XP_", "EXEC", "ONERROR", "ONLOAD", "ONCLICK", "ALERT", "<IFRAME", "</IFRAME"
        };

        public static bool IsSafe(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Normalizar entrada
            var sanitized = input.Trim().ToUpperInvariant();

            // Verificar palabras claves peligrosas
            if (_blacklist.Any(bad => sanitized.Contains(bad)))
                return false;

            // Detectar expresiones regulares peligrosas
            if (Regex.IsMatch(input, @"<[^>]*script[^>]*>", RegexOptions.IgnoreCase))
                return false;

            if (Regex.IsMatch(input, @"on\w+\s*=", RegexOptions.IgnoreCase)) // onerror=, onclick=, etc.
                return false;

            // Detectar caracteres invisibles o unicode peligrosos
            if (Regex.IsMatch(input, @"[\x00-\x1F\x7F\xA0\u200B\u200E\u200F\u2028\u2029]"))
                return false;

            return true;
        }
    }
}
