using Application.DTOs.OperationsSupervisor;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsSupervisor;
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
    public class CreateOperationsSupervisor(
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

        public async Task<BaseResponseId> ExecuteAsync(OperationsSupervisorCreateDto dto, long businessId)
        {
            // var validation = await _validator.ValidateAsync(dto);
            // if (!validation.IsValid)
            // {
            //     var errors = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
            //     throw new AppValidationException(errors);
            // }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<OperationSupervisor>(dto);

                var created = await _repository.CreateAsync(entity, businessId, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0)
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear el supervisor de operaciones en la BD.");
                }

                entity.SupervisorId = (long)created.Id;

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "OPERATIONS_SUPERVISOR",
                    RecordId = entity.SupervisorId,
                    CreateUser = entity.CreateUser
                };

                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

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
