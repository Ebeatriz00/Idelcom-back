using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.FileTrackingProducts
{
    public class FileTrackingProductsCreateDto
    {
        public long ProductsId { get; set; } 
        public string? FileTitle { get; set; }
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
    }
}
