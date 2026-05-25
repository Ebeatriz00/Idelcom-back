using Application.DTOs.PurchaseOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseOrder
{
    public class AttachPurchaseOrderInvoice(
        IPurchaseOrderRepository repository,
        IValidator<PurchaseOrderAttachInvoiceDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IValidator<PurchaseOrderAttachInvoiceDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponseId> ExecuteAsync(PurchaseOrderAttachInvoiceDto dto, long userId, long businessId)
        {
            await PurchaseOrderValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.PurchaseOrderId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Orden de compra", dto.PurchaseOrderId);

                var command = _mapper.Map<PurchaseOrderAttachInvoiceCommand>(dto);
                command.BusinessId = businessId;
                command.UserId = userId;

                var result = await _repository.AttachInvoiceAsync(command, transaction);

                var relation = result.Id is > 0
                    ? await _repository.GetPurchaseOrderInvoiceByIdAsync(businessId, result.Id.Value, transaction)
                    : null;

                if (relation is not null)
                {
                    var relationAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.PurchaseOrderInvoice,
                        relation.PurchaseOrderInvoiceId,
                        userId);

                    await _auditService.RegisterCreateAsync(
                        _mapper.Map<Core.Entities.Logistic.PurchaseOrderInvoice>(relation),
                        relationAuditLog,
                        transaction);
                }

                var after = await _repository.GetByIdAsync(businessId, dto.PurchaseOrderId, transaction);
                if (after?.Header is not null)
                {
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
                }

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
