using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationPasswordNoUppercaseException : BaseException
    {
        public ValidationPasswordNoUppercaseException(string message)
            : base("VALIDATION_PASSWORD_NO_UPPERCASE", 422, new[]
            {
                new GlobalErrorDetail(ErrorCodes.ValidationPasswordNoUppercase, message)
            })
        {
        }
    }
}
