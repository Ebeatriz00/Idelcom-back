using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;

namespace Application.UseCases.Operations.Support
{
    public class DeleteSupport(
        ISupportRepository repository,
        IAuditService auditService,
        ISqlConnectionFactory sqlConnectionFactory
    )
    {
        private readonly ISupportRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long supportId, long userId, long businessId)
        {
            var existing = await _repository.GetByIdAsync(supportId, businessId);
            if (existing == null) return null;

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = await _repository.DeleteAsync(supportId, userId, businessId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "SUPPORT",
                    RecordId = supportId,
                    CreateUser = userId
                };

                await _auditService.RegisterDeleteAsync(existing, auditLog, trans: transaction);
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
