using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationPasswordNoSymbolException : BaseException
    {
        public ValidationPasswordNoSymbolException(string message)
            : base("VALIDATION_PASSWORD_NO_SYMBOL", 422, new[]
            {
                new GlobalErrorDetail(ErrorCodes.ValidationPasswordNoSymbol, message)
            })
        {
        }
    }
}
