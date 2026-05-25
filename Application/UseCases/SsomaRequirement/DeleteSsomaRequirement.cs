using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.SsomaRequirement
{
    public class DeleteSsomaRequirement(
        ISsomaRequirementRepository repository,
        IAuditService auditService,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long requirementId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(requirementId, businessId);
                if (before == null)
                    throw new Exception("No se encontró el requerimiento SSOMA.");

                var deleted = await _repository.DeleteAsync(requirementId, businessId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = TableNames.SsomaRequirement,
                    RecordId = before.RequirementId,
                    CreateUser = userId
                };

                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

                transaction.Commit();
                return deleted;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al eliminar requerimientos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al eliminar requerimientos SSOMA.", ex.Message);
            }
        }
    }
}
