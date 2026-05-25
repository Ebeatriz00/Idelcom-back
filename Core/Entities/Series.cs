using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Series
    {
        public long SeriesId { get; set; }
        public long BusinessId { get; set; }
        public long PaymentTypeId { get; set; }
        public string SeriesName { get; set; } = string.Empty;
        public long Correlative { get; set; }
        public string Used { get; set; } = string.Empty;
        public int UsersBy {  get; set; }
        public string Status { get; set; } = "1";
        public int SerieCount { get; set; }
        public string PaymentTypeDescription { get; set; }
    }
}
