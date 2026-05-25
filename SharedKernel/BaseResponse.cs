using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class BaseResponseId
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public long? Id { get; set; }
    }

    public class BaseResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }
    }
}
