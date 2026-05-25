using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Exceptions
{
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public string? Details { get; }
        public IEnumerable<GlobalErrorDetail> Errors { get; }

        protected BaseException(string errorCode, int statusCode, IEnumerable<GlobalErrorDetail> errors, string? details = null)
            : base(details ?? errors?.FirstOrDefault()?.Message ?? errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
            Details = details;
        }
    }


}
