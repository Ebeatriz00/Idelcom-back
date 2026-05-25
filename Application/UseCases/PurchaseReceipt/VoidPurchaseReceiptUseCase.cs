using Application.Exceptions;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseReceipt
{
    public class VoidPurchaseReceiptUseCase(
        IPurchaseReceiptRepository repository,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseReceiptRepository _repository = repository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(long businessId, long purchaseReceiptId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, purchaseReceiptId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Recepcion de compra", purchaseReceiptId);

                var auditAction = before.Header.IsServiceReceipt
                    ? "Anulacion de conformidad de servicio"
                    : "Anulacion de recepcion de compra";

                var result = await _repository.VoidAsync(businessId, purchaseReceiptId, userId, transaction);
                var after = await _repository.GetByIdAsync(businessId, purchaseReceiptId, transaction);
                if (after?.Header is null)
                    throw new NotFoundException("Recepcion de compra", purchaseReceiptId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.PurchaseReceipt,
                    purchaseReceiptId,
                    userId,
                    auditAction);

                await _auditService.RegisterUpdateAsync(
                    _mapper.Map<Core.Entities.Logistic.PurchaseReceipt>(before.Header),
                    _mapper.Map<Core.Entities.Logistic.PurchaseReceipt>(after.Header),
                    auditLog,
                    transaction);

                foreach (var beforeDetail in _mapper.Map<List<PurchaseReceiptDetail>>(before.Details))
                {
                    beforeDetail.BusinessId = businessId;

                    var detailAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.PurchaseReceiptDetail,
                        beforeDetail.PurchaseReceiptDetailId,
                        userId,
                        auditAction);

                    await _auditService.RegisterDeleteAsync(beforeDetail, detailAuditLog, transaction);
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
