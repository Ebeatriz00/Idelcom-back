using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Suppliers
{
    public class SuppliersStatusToggleDto
    {
        public long SuppliersId { get; set; }
        public string Status { get; set; } = "1";
    }
}
