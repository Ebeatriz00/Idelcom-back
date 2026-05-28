using SharedKernel;
using SharedKernel.Constants;

namespace Application.Exceptions
{
    public sealed class AuthInvalidCredentialsException : BaseException
    {
        public int? AttemptsRemaining { get; }

        public AuthInvalidCredentialsException(string? message = null, int? attemptsRemaining = null)
            : base("AUTH_INVALID_CREDENTIALS", 401,
                   new[] { new GlobalErrorDetail(ErrorCodes.InvalidCredential, message ?? "Usuario o contraseña inválidos.") })
        {
            AttemptsRemaining = attemptsRemaining;
        }
    }
}
