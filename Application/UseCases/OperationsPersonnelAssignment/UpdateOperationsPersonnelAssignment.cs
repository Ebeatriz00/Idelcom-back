using Application.DTOs.OperationsPersonnelAssignment;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsPersonnelAssignment;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsPersonnelAssignment;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelAssignment
{
    public class UpdateOperationsPersonnelAssignment(
            IOperationsPersonnelAssignmentRepository repository,
            IAuditService auditService,
            IMapper mapper,
            ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly IOperationsPersonnelAssignmentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(OperationsPersonnelAssignmentUpdateDto dto, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Obtenemos el estado actual antes de actualizar para la auditoría
                var before = await _repository.GetByIdAsync(dto.AssignmentId);
                if (before == null)
                    throw new Exception("No se encontró la asignación de personal.");

                var entity = _mapper.Map<OperationPersonnelAssignment>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var result = await _repository.UpdateAsync(entity, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "OPERATIONS_PERSONNEL_ASSIGNMENT",
                    RecordId = entity.AssignmentId,
                    CreateUser = userId
                };

                // Pasamos 'before' (estado anterior) y 'entity' (estado nuevo)
                await _auditService.RegisterUpdateAsync(before, entity, auditLog, trans: transaction);

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
