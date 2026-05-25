using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(IEnumerable<GlobalErrorDetail> errors)
            : base("VALIDATION_ERROR", 400, errors)
        {
        }
    }


}
