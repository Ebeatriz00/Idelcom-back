using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base("CONFLICT", 409, new[]
            {
                new GlobalErrorDetail(ErrorCodes.Conflict, message)
            })
        {
        }
    }
}
