using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class DeleteSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IAuditService auditService,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long ssomaHomologationPersonnelDocumentId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(ssomaHomologationPersonnelDocumentId, businessId);
                if (before == null)
                    throw new BusinessException("No se encontró el documento de homologación de personal SSOMA.");

                var deleted = await _repository.DeleteAsync(ssomaHomologationPersonnelDocumentId, businessId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = TableNames.SsomaHomologationPersonnelDocument,
                    RecordId = before.SsomaHomologationPersonnelDocumentId,
                    CreateUser = userId
                };

                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

                transaction.Commit();
                return deleted;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al eliminar el documento de homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al eliminar el documento de homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
