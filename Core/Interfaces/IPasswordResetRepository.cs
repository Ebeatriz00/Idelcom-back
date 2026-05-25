using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task<PasswordReset> CreateAsync(string tenantId, Guid userId, string token, DateTime expiresAtUtc);
        Task<PasswordReset?> GetByTokenAsync(string tenantId, string token);
        Task ConsumeAsync(Guid id);
    }
}
