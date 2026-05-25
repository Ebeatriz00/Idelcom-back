using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Series
{
    public class SeriesResponseDto
    {
        public long SeriesId { get; set; }
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public string SeriesName { get; set; } = string.Empty;
        public long Correlative { get; set; }
        public string Used { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public string PaymentTypeDescription { get; set; } = string.Empty;
        public int SerieCount { get; set; }
    }
}
