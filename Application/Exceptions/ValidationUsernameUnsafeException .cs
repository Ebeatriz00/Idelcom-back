using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationUsernameUnsafeException : BaseException
    {
        public ValidationUsernameUnsafeException(string message)
            : base("VALIDATION_USERNAME_UNSAFE", 422, new[]
                {
                    new GlobalErrorDetail(ErrorCodes.ValidationUsernameUnsafe, message)
                })
            {
        }
    }
}
