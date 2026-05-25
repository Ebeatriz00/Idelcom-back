using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class AuthDtos
    {
        public record RequestResetCommand(string Email, string? Tenant);
        public record ResendOtpCommand(string Email);
        public record VerifyOtpCommand(string Email, string Code);
        public record BootstrapByTokenQuery(string Token, string? Tenant);
        public record ResetPasswordCommand(string ResetToken, string NewPassword, string? Email, string? Tenant);
        public record AuthResponse(string AccessToken, string RefreshToken, DateTime RefreshExpiresAt);
        public record OkResponse(bool Ok = true, int? CooldownSeconds = null);
        public record BootstrapResponse(string ResetToken, string? Email);
    }
}
