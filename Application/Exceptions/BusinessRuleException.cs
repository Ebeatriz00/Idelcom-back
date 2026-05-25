using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class BusinessRuleException : BaseException
    {
        public BusinessRuleException(string message)
            : base("BUSINESS_RULE_VIOLATION", 422, new[]
            {
                new GlobalErrorDetail(ErrorCodes.BusinessRuleViolation, message)
            })
        {
        }
    }

}
