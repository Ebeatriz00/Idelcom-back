using SharedKernel;
using SharedKernel.Constants;

namespace Application.Exceptions
{
    public class QuotationFileFoundException : BaseException
    {
        public QuotationFileFoundException(string message)
            : base(
                "QUOTATION_FILE_FOUND",
                422,
                new[]
                {
                    new GlobalErrorDetail(ErrorCodes.QuoteFileFound, message)
                }
            )
        {
        }
    }
}
