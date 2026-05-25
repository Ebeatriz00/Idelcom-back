using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string userMessage, string? technicalDetails = null)
            : base("BUSINESS_ERROR", 400, new[]
            {
                new GlobalErrorDetail(ErrorCodes.BusinessRuleViolation, userMessage)
            }, technicalDetails)
        {
        }
    }
}
