using Application.DTOs.PurchaseReceipt;
using Application.Exceptions;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseReceipt
{
    public class RegularizePurchaseReceiptWithPurchaseOrderUseCase(
        IPurchaseReceiptRepository repository,
        IValidator<PurchaseReceiptRegularizeDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseReceiptRepository _repository = repository;
        private readonly IValidator<PurchaseReceiptRegularizeDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(PurchaseReceiptRegularizeDto dto, long userId, long businessId)
        {
            await PurchaseReceiptValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.PurchaseReceiptId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Recepcion de compra", dto.PurchaseReceiptId);

                if (before.Header.IsServiceReceipt)
                    throw new BusinessException("La regularización posterior solo aplica a recepciones de mercadería sin OC.");

                var command = _mapper.Map<PurchaseReceiptRegularizeCommand>(dto);
                command.BusinessId = businessId;
                command.PurchaseReceiptId = dto.PurchaseReceiptId;
                command.UserId = userId;

                var result = await _repository.RegularizeWithPurchaseOrderAsync(command, transaction);

                var after = await _repository.GetByIdAsync(businessId, dto.PurchaseReceiptId, transaction);
                if (after?.Header is null)
                    throw new NotFoundException("Recepcion de compra", dto.PurchaseReceiptId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.PurchaseReceipt,
                    dto.PurchaseReceiptId,
                    userId,
                    "Regularizacion con orden de compra");

                await _auditService.RegisterUpdateAsync(
                    _mapper.Map<Core.Entities.Logistic.PurchaseReceipt>(before.Header),
                    _mapper.Map<Core.Entities.Logistic.PurchaseReceipt>(after.Header),
                    auditLog,
                    transaction);

                var beforeDetails = _mapper.Map<List<PurchaseReceiptDetail>>(before.Details)
                    .Select(x =>
                    {
                        x.BusinessId = businessId;
                        return x;
                    })
                    .ToDictionary(x => x.PurchaseReceiptDetailId);

                foreach (var afterDetail in _mapper.Map<List<PurchaseReceiptDetail>>(after.Details))
                {
                    afterDetail.BusinessId = businessId;

                    if (!beforeDetails.TryGetValue(afterDetail.PurchaseReceiptDetailId, out var beforeDetail))
                        continue;

                    var detailAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.PurchaseReceiptDetail,
                        afterDetail.PurchaseReceiptDetailId,
                        userId,
                        "Regularizacion con orden de compra");

                    await _auditService.RegisterUpdateAsync(beforeDetail, afterDetail, detailAuditLog, transaction);
                }

                transaction.Commit();
                return result;
            }
            catch
            {
                TryRollback(transaction);
                throw;
            }
        }

        private static void TryRollback(System.Data.IDbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
            }
        }
    }
}
