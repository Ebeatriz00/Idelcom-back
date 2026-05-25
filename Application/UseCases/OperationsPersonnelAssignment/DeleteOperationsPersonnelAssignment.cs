using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsPersonnelAssignment;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelAssignment
{
    public class DeleteOperationsPersonnelAssignment(
            IOperationsPersonnelAssignmentRepository repository,
            IAuditService auditService,
            ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly IOperationsPersonnelAssignmentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long assignmentId, long userId, long businessId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Obtenemos el estado actual antes de borrarlo para la auditoría
                var before = await _repository.GetByIdAsync(assignmentId);
                if (before == null)
                    throw new Exception("No se encontró la asignación de personal para eliminar.");

                var result = await _repository.DeleteAsync(assignmentId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "OPERATIONS_PERSONNEL_ASSIGNMENT",
                    RecordId = assignmentId,
                    CreateUser = userId
                };

                // Agregamos el parámetro 'before' que faltaba
                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

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
