using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Email
{
    public sealed class SmtpSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "ERP Notificaciones";
    }
}
