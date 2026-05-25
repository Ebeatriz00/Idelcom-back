using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.FileTrackingProducts
{
    public class FileTrackingProductsDeleteDto
    {
        public long FileTrackingProductsId { get; set; } 
        public long ProductsId { get; set; }
    }
}
