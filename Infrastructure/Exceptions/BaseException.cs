using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public string? Details { get; }
        public IEnumerable<GlobalErrorDetail> Errors { get; }

        protected BaseException(string errorCode, int statusCode, IEnumerable<GlobalErrorDetail> errors, string? details = null)
            : base(errors?.FirstOrDefault()?.Message ?? "Error inesperado.")
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
            Details = details;
        }
    }
}
