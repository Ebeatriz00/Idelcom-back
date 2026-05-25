using Core.Entities.Audit;

namespace Core.Interfaces.Audit
{
    public interface IAuditLogFactory
    {
        AuditLog Create(
            long businessId,
            string tableName,
            long recordId,
            long userId,
            string? comment = null);
    }
}
