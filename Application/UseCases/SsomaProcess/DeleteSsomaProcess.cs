using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaProcess
{
    public class DeleteSsomaProcess(
        ISsomaProcessRepository repository,
        IAuditService auditService,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly ISsomaProcessRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<GlobalResponse> ExecuteAsync(long ssomaProcessId, long operationsId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(ssomaProcessId, operationsId, businessId);

                if (before == null)
                    throw new Exception("No se encontró el proceso ssoma.");

                var updated = await _repository.DeleteAsync(ssomaProcessId, userId, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = before.BusinessId,
                    TableName = TableNames.SsomaProcess,
                    RecordId = before.OperationsId,
                    CreateUser = userId
                };

                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

                transaction.Commit();
                return updated;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al elimnar procesos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al elimar processos SSOMA.", ex.Message);
            }
        }
    }
}
