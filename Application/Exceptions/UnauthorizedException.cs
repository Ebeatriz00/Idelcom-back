using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "No autorizado.")
            : base("UNAUTHORIZED", 401, new[]
            {
                new GlobalErrorDetail(ErrorCodes.Unauthorized, message)
            })
        {
        }
    }
}
