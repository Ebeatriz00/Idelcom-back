using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationEmailInvalidException : BaseException
    {
        public ValidationEmailInvalidException(string message)
            : base("VALIDATION_EMAIL_INVALID", 422, new[]
            {
                new GlobalErrorDetail(ErrorCodes.ValidationEmailInvalid, message)
            })
        {
        }
    }
}
