using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class GlobalErrorDetail
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public GlobalErrorDetail(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
