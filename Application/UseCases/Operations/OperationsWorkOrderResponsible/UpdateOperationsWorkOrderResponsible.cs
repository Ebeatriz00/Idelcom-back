using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.Operations.Core.Entities;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrderResponsible
{
    public class UpdateOperationsWorkOrderResponsible(
        IOperationsWorkOrderResponsibleRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsWorkOrderResponsibleUpdateDto> validator)
    {
        private readonly IOperationsWorkOrderResponsibleRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsWorkOrderResponsibleUpdateDto> _validator = validator;

        public async Task<BaseResponse> ExecuteAsync(
            OperationsWorkOrderResponsibleUpdateDto dto,
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
                var before = await _repository.GetByIdAsync(dto.WorkOrderResponsibleId, businessId);

                if (before == null)
                    throw new NotFoundException("Operations Work Order Responsible", dto.WorkOrderResponsibleId);

                var entity = _mapper.Map<OperationWorkOrderResponsible>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);
                var after = await _repository.GetByIdAsync(dto.WorkOrderResponsibleId, businessId, transaction);

                if (after == null)
                    throw new NotFoundException("Operations Work Order Responsible", dto.WorkOrderResponsibleId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrderResponsible,
                    before.WorkOrderResponsibleId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                await transaction.CommitAsync();
                return updated;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
