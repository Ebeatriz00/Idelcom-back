using Application.DTOs.OperationsPersonnelMovement;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsPersonnelMovement;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsPersonnelMovement;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelMovement
{
    public class CreateOperationPersonnelMovement(
            IOperationsPersonnelMovementRepository repository,
            IAuditService auditService,
            IMapper mapper,
            ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly IOperationsPersonnelMovementRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponseId> ExecuteAsync(OperationPersonnelMovementCreateDto dto, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<OperationPersonnelMovement>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0)
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear el movimiento de personal en la BD.");
                }

                entity.MovementId = (long)created.Id;

                var auditLog = new AuditLog
                {
                    BusinessId = entity.BusinessId,
                    TableName = "OPERATIONS_PERSONNEL_MOVEMENT",
                    RecordId = entity.MovementId,
                    CreateUser = entity.CreateUser
                };

                await _auditService.RegisterCreateAsync<OperationPersonnelMovement>(entity, auditLog, trans: transaction);

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
