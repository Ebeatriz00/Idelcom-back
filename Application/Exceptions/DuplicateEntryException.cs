using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class DuplicateEntryException : BaseException
    {
        public DuplicateEntryException(string message)
            : base("DUPLICATE_ENTRY", 409, new[]
            {
                new GlobalErrorDetail(ErrorCodes.DuplicateEntry, message)
            })
        {
        }
    }
}
