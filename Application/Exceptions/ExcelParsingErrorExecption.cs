using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
        public class ExcelParsingErrorExecption : BaseException
        {
            public ExcelParsingErrorExecption(string message, Exception? inner = null)
                : base(
                      "EXCEL_PARSING_ERROR",
                      422,
                      new[]
                      {
                      new GlobalErrorDetail(ErrorCodes.ExcelParsingError, message)
                      }
                  )
            {
                
            }
        }
}
