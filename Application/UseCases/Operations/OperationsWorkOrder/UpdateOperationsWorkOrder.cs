using Application.DTOs.Operations.OperationsWorkOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.Operations;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class UpdateOperationsWorkOrder(
        IOperationsWorkOrderRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsWorkOrderUpdateDto> validator
        )
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsWorkOrderUpdateDto> _validator = validator;

        public async Task<BaseResponse> ExecuteAsync(OperationsWorkOrderUpdateDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.WorkOrderId, businessId);

                if (before == null)
                    throw new NotFoundException("Operations Work Order", dto.WorkOrderId);

                var entity = _mapper.Map<OperationWorkOrder>(dto);

                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);
                var after = await _repository.GetByIdAsync(dto.WorkOrderId, businessId, transaction);

                if (after == null)
                    throw new NotFoundException("Operations Work Order", dto.WorkOrderId);

                var audilog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrder,
                    before.WorkOrderId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, audilog, transaction);

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
