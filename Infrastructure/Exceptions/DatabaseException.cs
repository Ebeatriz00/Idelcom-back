using SharedKernel;
using SharedKernel.Constants;

namespace Infrastructure.Exceptions
{
    public class DatabaseException(string userMessage, string? technicalDetails = null) : BaseException("DATABASE_ERROR", 500,
            [
                new GlobalErrorDetail(ErrorCodes.DatabaseExecutionError, userMessage)
            ], technicalDetails)
    {
    }

}
