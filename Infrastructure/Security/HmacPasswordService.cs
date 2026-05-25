using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class HmacPasswordService : IPasswordService
    {
        public string HashPassword(string plainPassword, out string salt)
        {
            using var hmac = new HMACSHA512();
            salt = Convert.ToBase64String(hmac.Key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string plainPassword, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            return Convert.ToBase64String(computedHash) == storedHash;
        }
    }
}
