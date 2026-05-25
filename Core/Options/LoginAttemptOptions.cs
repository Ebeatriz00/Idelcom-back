using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Options
{
    public class LoginAttemptOptions
    {
        public int MaxAttempts { get; set; } = 3;
        public int WindowMinutes { get; set; } = 10; // ventana para contar fallos
        public int LockoutMinutes { get; set; } = 5; // tiempo bloqueado
    }
}
