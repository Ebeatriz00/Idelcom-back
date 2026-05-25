using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Bank
{
    public class BankUpdateDto
    {
        public long BankId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public long UsersBy { get; set; }
    }
}
