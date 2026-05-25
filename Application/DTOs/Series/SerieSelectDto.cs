using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Series
{
    public class SeriesSelectDto
    {
        public long SeriesId { get; set; }
        public string SeriesName { get; set; } = string.Empty;
    }
}
