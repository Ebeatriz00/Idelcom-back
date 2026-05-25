using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class AuthLockedOutException : BaseException
    {
        public TimeSpan? RetryAfter { get; }

        public AuthLockedOutException(string? message = null, TimeSpan? retryAfter = null)
            : base("AUTH_LOCKED_OUT", 429,
                   new[] { new GlobalErrorDetail(ErrorCodes.AuthLocked, message ?? "Demasiados intentos. Intenta más tarde.") })
        {
            RetryAfter = retryAfter;
        }
    }
}
