using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Currency
{
    public class CurrencyCreateDto
    {
        public long BusinessId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CodeSunat { get; set; } = string.Empty;
        public string symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UsersBy { get; set; }

    }
}
