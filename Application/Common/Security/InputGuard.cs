using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common.Security
{
    public static class InputGuard
    {
        private static readonly Regex ZeroWidth = new(@"\p{Cf}", RegexOptions.Compiled);
        private static readonly Regex Control = new(@"[\p{Cc}]", RegexOptions.Compiled);
        private static readonly Regex SqlTokens = new(@"(--|/\*|\*/|;)", RegexOptions.Compiled);

        // Acepta letras, dígitos, -, _, ., + y @  (usernames, dominios-like, emails)
        private static readonly Regex SafeUserKey = new(@"^[\p{L}\p{Nd}\-_.+@]+$", RegexOptions.Compiled);

        public static string NormalizeKC(string s) => s.Normalize(NormalizationForm.FormKC);
        public static bool HasControlOrZeroWidth(string s) => Control.IsMatch(s) || ZeroWidth.IsMatch(s);
        public static bool ContainsDangerousSqlTokens(string s) => SqlTokens.IsMatch(s);
        public static bool IsSafeUserKey(string s) => SafeUserKey.IsMatch(s);
        public static bool IsDigits(string s) => s.All(char.IsDigit);
    }
}
