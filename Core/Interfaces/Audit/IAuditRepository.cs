using Core.Entities.Audit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Audit
{
    public interface IAuditRepository
    {
        Task<long> InsertAuditLogAsync(AuditLog auditLog, IDbTransaction? transaction = null);

        Task InsertAuditLogDetailAsync(long auditLogId, long userId, long businessId, List<AuditLogDetail> details, IDbTransaction? transaction = null);
    }
}
