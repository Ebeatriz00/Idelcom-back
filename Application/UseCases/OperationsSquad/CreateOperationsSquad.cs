using Application.DTOs.OperationsSquad;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsSquad;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsSquad;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSquad
{
    public class CreateOperationSquad(
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

        public async Task<BaseResponseId> ExecuteAsync(OperationsSquadCreateDto dto)
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
                var entity = _mapper.Map<OperationSquad>(dto);

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0) 
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear la cuadrilla en la BD.");
                }

                entity.SquadId = (long)created.Id;

                var auditLog = new AuditLog
                {
                    BusinessId = entity.BusinessId,
                    TableName = "OPERATION_SQUAD", 
                    RecordId = entity.SquadId,
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
