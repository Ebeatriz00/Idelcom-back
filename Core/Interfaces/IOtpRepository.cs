using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOtpRepository
    {
        Task<OtpCode> UpsertAsync(string tenantId, Guid userId, string code, DateTime expiresAtUtc);
        Task<OtpCode?> GetActiveAsync(string tenantId, Guid userId);
        Task ConsumeAsync(Guid id);
        Task IncrementAttemptsAsync(Guid id);
    }
}
