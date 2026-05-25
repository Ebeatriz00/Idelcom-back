using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Bank
    {
        public long BankId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
        public int BankCount { get; set; }  


    }
}
