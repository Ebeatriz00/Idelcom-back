using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class InvalidCharacterException : BaseException
    {
        public InvalidCharacterException(string message)
            : base("INVALID_CHARACTER", 400, new[]
            {
                new GlobalErrorDetail(SharedKernel.Constants.ErrorCodes.ValidationIllegalChar, message)
            })
        {
        }
    }
}
