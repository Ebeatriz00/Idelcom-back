using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsSquad;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSquad
{
    namespace Application.UseCases.OperationsSquad
    {
        public class DeleteOperationsSquad(
            IOperationsSquadRepository repository,
            IAuditService auditService,
            IMapper mapper,
            ISqlConnectionFactory sqlConnectionFactory
            )
        {
            private readonly IOperationsSquadRepository _repository = repository;
            private readonly IAuditService _auditService = auditService;
            private readonly IMapper _mapper = mapper;
            private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

            public async Task<BaseResponse> ExecuteAsync(long squadId, long userId)
            {
                using var connection = _sqlConnectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var before = await _repository.GetByIdAsync(squadId);

                    if (before == null)
                        throw new Exception("No se encontró la cuadrilla.");

                    var updated = await _repository.DeleteAsync(squadId, userId, transaction);

                    var auditLog = new AuditLog
                    {
                        BusinessId = before.BusinessId,
                        TableName = TableNames.OperationsSquad,
                        RecordId = before.SquadId,
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
            }
        }
    }
}
