using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    public class ExternalServiceException : BaseException
    {
        public ExternalServiceException(string serviceName, string? technicalDetails = null)
            : base("EXTERNAL_SERVICE_ERROR", 503, new[]
            {
                new GlobalErrorDetail(ErrorCodes.ExternalApiFailed, $"Error al comunicarse con el servicio externo: {serviceName}")
            }, technicalDetails)
        {
        }
    }

}
