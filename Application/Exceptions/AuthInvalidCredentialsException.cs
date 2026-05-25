using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class AuthInvalidCredentialsException : BaseException
    {
        public AuthInvalidCredentialsException(string? message = null)
            : base("AUTH_INVALID_CREDENTIALS", 401,
                   new[] { new GlobalErrorDetail(ErrorCodes.InvalidCredential, message ?? "Usuario o contraseña inválidos.") })
        { }
    }

}
