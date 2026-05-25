using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.OperationsSquad;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.OperationsSquad;
using Core.Interfaces.Audit;
using Core.Interfaces.OperationsSquad;

using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.OperationsSquad
{
    public class UpdateOperationsSquad(
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
        //private readonly IValidator<OperationsSquadUpdateDto> _validator = validator;
        
        public async Task<BaseResponse> ExecuteAsync(OperationsSquadUpdateDto dto)
        {
         //var validation = await _validator.ValidateAsync(dto);
         //if (!validation.IsValid)
         //{
         //    var errors = validation.Errors
         //        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
         //        .ToList();
         //    throw new AppValidationException(errors);
         //}
         
         using var connection = _sqlConnectionFactory.CreateConnection();
         await connection.OpenAsync();
         using var transaction = connection.BeginTransaction();
            try
            {
                var before = await _repository.GetByIdAsync(dto.SquadId);
                if (before == null)
                    throw new Exception("No se encontró la cuadrilla.");
                var entity = _mapper.Map<OperationSquad>(dto);
                var updated = await _repository.UpdateAsync(entity, transaction);
                var auditLog = new AuditLog
                {
                    BusinessId = before.BusinessId,
                    TableName = TableNames.OperationsSquad,
                    RecordId = before.SquadId,
                    CreateUser = dto.UpdateUser
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
}
