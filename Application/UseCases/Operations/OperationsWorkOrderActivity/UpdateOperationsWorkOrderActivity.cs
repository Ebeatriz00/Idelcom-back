using Application.DTOs.Operations.OperationsWorkOrderActivity;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.Operations;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class UpdateOperationsWorkOrderActivity(
        IOperationsWorkOrderActivityRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsWorkOrderActivityUpdateDto> validator)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsWorkOrderActivityUpdateDto> _validator = validator;

        public async Task<BaseResponse> ExecuteAsync(
            OperationsWorkOrderActivityUpdateDto dto,
            long userId,
            long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(dto.ActivityId, businessId, transaction);
                if (before == null)
                    throw new InvalidOperationException("No se encontró la actividad de la orden de trabajo.");

                var entity = _mapper.Map<OperationWorkOrderActivity>(dto);
                var result = await _repository.UpdateAsync(entity, userId, businessId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrderActivity,
                    dto.ActivityId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, entity, auditLog, trans: transaction);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
