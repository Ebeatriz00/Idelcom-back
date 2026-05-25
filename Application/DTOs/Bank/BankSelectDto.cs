using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Bank
{
    public class BankSelectDto
    {
        public long BankId { get; set; }
        public string Abrv { get; set; } = string.Empty;
    }
}
