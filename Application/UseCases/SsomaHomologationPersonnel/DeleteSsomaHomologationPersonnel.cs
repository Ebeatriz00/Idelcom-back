using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class DeleteSsomaHomologationPersonnel(
        ISsomaHomologationPersonnelRepository repository,
        IAuditService auditService,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long homologationPersonnelId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(homologationPersonnelId, businessId);
                if (before == null)
                    throw new BusinessException("No se encontró la homologación de personal SSOMA.");

                var deleted = await _repository.DeleteAsync(homologationPersonnelId, businessId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = TableNames.SsomaHomologationPersonnel,
                    RecordId = before.HomologationPersonnelId,
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
                throw new DatabaseException("Error al eliminar la homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al eliminar la homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
