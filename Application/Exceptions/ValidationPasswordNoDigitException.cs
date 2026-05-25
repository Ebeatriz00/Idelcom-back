using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationPasswordNoDigitException : BaseException
    {
        public ValidationPasswordNoDigitException(string message)
            : base("VALIDATION_PASSWORD_NO_DIGIT", 422, new[]
            {
                new GlobalErrorDetail(ErrorCodes.ValidationPasswordNoDigit, message)
            })
        {
        }
    }

}
