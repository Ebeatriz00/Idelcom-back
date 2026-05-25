using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string entityName, object key)
            : base("NOT_FOUND", 404, new[]
            {
                new GlobalErrorDetail(
                    ErrorCodes.EntityNotFound,
                    $"{entityName} con ID {key} no fue encontrado.")
            })
        {
        }
    }

}
