using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<Auth> AuthenticateAsync(string usersKeyNormalized, CancellationToken ct = default);
    }
}
