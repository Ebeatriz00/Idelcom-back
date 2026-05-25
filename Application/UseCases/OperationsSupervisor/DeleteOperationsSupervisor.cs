using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsSupervisor;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSupervisor
{
    public class DeleteOperationsSupervisor(
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

        public async Task<BaseResponse> ExecuteAsync(long supervisorId, long userId, long businessId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(supervisorId);

                if (before == null)
                    throw new Exception("No se encontró el supervisor de operaciones.");

                var updated = await _repository.DeleteAsync(supervisorId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "OPERATIONS_SUPERVISOR",
                    RecordId = before.SupervisorId,
                    CreateUser = userId
                };

                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

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
}
