using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class RefreshTokenExpiredException : BaseException
    {
        public RefreshTokenExpiredException(string? msg = null)
            : base("REFRESH_TOKEN_EXPIRED", 401,
                new[] { new GlobalErrorDetail(ErrorCodes.RefreshTokenExpired, msg ?? "Refresh token expirado.") })
        { }
    }
}
