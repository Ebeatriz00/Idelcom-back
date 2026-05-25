using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Series
{
    public class SeriesUpdateDto
    {
        public long SeriesId { get; set; }
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public string SeriesName { get; set; } = string.Empty;
        public long Correlative { get; set; }
        public string Used { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
