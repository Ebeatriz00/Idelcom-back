using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductLines
{
    public class ProductLinesByIdDto : BaseAuditableEntity
    {
        public long BusinessId { get; set; }
        public long ProductLinesId { get; set; } 
        public string Description { get; set; } = string.Empty;
        public long CategoriesId { get; set; }

    }
}
