using Application.DTOs.PurchaseOrder;
using Application.Exceptions;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseOrder
{
    public class UpdatePurchaseOrder(
        IPurchaseOrderRepository repository,
        IValidator<PurchaseOrderUpdateDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IValidator<PurchaseOrderUpdateDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(PurchaseOrderUpdateDto dto, long userId, long businessId)
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

                var command = _mapper.Map<PurchaseOrderCommand>(dto);
                command.BusinessId = businessId;
                command.UserId = userId;

                var result = await _repository.UpdateAsync(command, transaction);
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

                await AuditDetailsAsync(before, after, businessId, userId, transaction);

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task AuditDetailsAsync(
            Core.Projections.Logistic.PurchaseOrderByIdProjection before,
            Core.Projections.Logistic.PurchaseOrderByIdProjection after,
            long businessId,
            long userId,
            System.Data.IDbTransaction transaction)
        {
            var beforeDetails = MapDetailEntities(before)
                .ToDictionary(x => x.PurchaseOrderDetailId);

            var afterDetails = MapDetailEntities(after)
                .ToDictionary(x => x.PurchaseOrderDetailId);

            foreach (var afterDetail in afterDetails.Values)
            {
                var detailAuditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.PurchaseOrderDetail,
                    afterDetail.PurchaseOrderDetailId,
                    userId);

                if (!beforeDetails.TryGetValue(afterDetail.PurchaseOrderDetailId, out var beforeDetail))
                {
                    await _auditService.RegisterCreateAsync(afterDetail, detailAuditLog, transaction);
                    continue;
                }

                if (beforeDetail.IsActive && !afterDetail.IsActive)
                {
                    await _auditService.RegisterDeleteAsync(beforeDetail, detailAuditLog, transaction);
                    continue;
                }

                await _auditService.RegisterUpdateAsync(beforeDetail, afterDetail, detailAuditLog, transaction);
            }

            foreach (var beforeDetail in beforeDetails.Values.Where(x => !afterDetails.ContainsKey(x.PurchaseOrderDetailId)))
            {
                var detailAuditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.PurchaseOrderDetail,
                    beforeDetail.PurchaseOrderDetailId,
                    userId);

                await _auditService.RegisterDeleteAsync(beforeDetail, detailAuditLog, transaction);
            }
        }

        private List<PurchaseOrderDetail> MapDetailEntities(Core.Projections.Logistic.PurchaseOrderByIdProjection source)
        {
            var details = _mapper.Map<List<PurchaseOrderDetail>>(source.Details);
            foreach (var detail in details)
            {
                detail.BusinessId = source.Header?.BusinessId ?? 0;
                detail.PurchaseOrderId = source.Header?.PurchaseOrderId ?? 0;
            }

            return details;
        }
    }
}
