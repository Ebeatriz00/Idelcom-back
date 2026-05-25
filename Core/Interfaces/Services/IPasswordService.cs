using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IPasswordService
    {
        string HashPassword(string plainPassword, out string salt);
        bool VerifyPassword(string plainPassword, string hashedPassword, string salt);
    }
}
