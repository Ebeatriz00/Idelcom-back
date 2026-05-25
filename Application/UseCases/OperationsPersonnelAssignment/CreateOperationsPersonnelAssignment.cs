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
    public class CreateOperationsPersonnelAssignment(
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

        public async Task<BaseResponseId> ExecuteAsync(OperationsPersonnelAssignmentCreateDto dto, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<OperationPersonnelAssignment>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0)
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear la asignación de personal en la BD.");
                }

                entity.AssignmentId = (long)created.Id;

                var auditLog = new AuditLog
                {
                    BusinessId = entity.BusinessId,
                    TableName = "OPERATIONS_PERSONNEL_ASSIGNMENT",
                    RecordId = entity.AssignmentId,
                    CreateUser = entity.CreateUser
                };

                await _auditService.RegisterCreateAsync<OperationPersonnelAssignment>(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
