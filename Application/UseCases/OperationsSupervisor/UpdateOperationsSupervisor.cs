using Application.DTOs.OperationsSupervisor;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsSupervisor;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsSupervisor;
using Infrastructure.Persistence;
using SharedKernel;

public class UpdateOperationsSupervisor(
            IOperationsSupervisorRepository repository,
            IAuditService auditService,
            IMapper mapper,
            ISqlConnectionFactory sqlConnectionFactory
            )
{
    private readonly IOperationsSupervisorRepository _repository = repository;
    private readonly IAuditService _auditService = auditService;
    private readonly IMapper _mapper = mapper;
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<BaseResponse> ExecuteAsync(OperationsSupervisorUpdateDto dto, long businessId)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var before = await _repository.GetByIdAsync(dto.SupervisorId);

            if (before == null)
                throw new Exception("No se encontró el supervisor de operaciones.");

            var entity = _mapper.Map<OperationSupervisor>(dto);

            var updated = await _repository.UpdateAsync(entity, businessId, transaction);

            var auditLog = new AuditLog
            {
                BusinessId = businessId,
                TableName = "OPERATIONS_SUPERVISOR",
                RecordId = before.SupervisorId,
                CreateUser = dto.UpdateUser,
            };

            await _auditService.RegisterUpdateAsync(before, entity, auditLog, trans: transaction);

            transaction.Commit();
            return updated;
        }
        catch (InvalidCastException ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException(ex.Message);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
    