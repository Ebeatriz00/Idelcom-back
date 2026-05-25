using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public  class RefreshTokenInvalidException : BaseException
    {
        public RefreshTokenInvalidException(string? msg = null)
            : base("REFRESH_TOKEN_INVALID", 401,
                new[] { new GlobalErrorDetail(ErrorCodes.RefreshTokenInvalid, msg ?? "Refresh token inválido.") })
        { }
    }
}
