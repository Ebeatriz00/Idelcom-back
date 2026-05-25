using Application.DTOs.PurchaseOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseOrder
{
    public class ApprovePurchaseOrder(
        IPurchaseOrderRepository repository,
        IValidator<PurchaseOrderApproveDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IValidator<PurchaseOrderApproveDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(PurchaseOrderApproveDto dto, long userId, long businessId)
        {
            dto.ApprovedBy = dto.ApprovedBy <= 0 ? userId : dto.ApprovedBy;
            await PurchaseOrderValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.PurchaseOrderId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Orden de compra", dto.PurchaseOrderId);

                var result = await _repository.ApproveAsync(businessId, dto.PurchaseOrderId, dto.ApprovedBy, userId, transaction);
                var after = await _repository.GetByIdAsync(businessId, dto.PurchaseOrderId, transaction);
                if (after?.Header is null)
                    throw new NotFoundException("Orden de compra", dto.PurchaseOrderId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.PurchaseOrder,
                    dto.PurchaseOrderId,
                    userId);

                await _auditService.RegisterUpdateAsync(
                    _mapper.Map<Core.Entities.Logistic.PurchaseOrder>(before.Header),
                    _mapper.Map<Core.Entities.Logistic.PurchaseOrder>(after.Header),
                    auditLog,
                    transaction);

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
