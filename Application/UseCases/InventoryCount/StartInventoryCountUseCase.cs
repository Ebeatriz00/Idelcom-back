using Application.Exceptions;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.InventoryCount
{
    public class StartInventoryCountUseCase(
        IInventoryCountRepository repository,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IInventoryCountRepository _repository = repository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(long inventoryCountId, long userId, long businessId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, inventoryCountId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Toma de inventario", inventoryCountId);

                var result = await _repository.StartAsync(businessId, inventoryCountId, userId, transaction);

                var after = await _repository.GetByIdAsync(businessId, inventoryCountId, transaction);
                if (after?.Header is null)
                    throw new NotFoundException("Toma de inventario", inventoryCountId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.InventoryCount,
                    inventoryCountId,
                    userId,
                    "Inicio de conteo de inventario");

                await _auditService.RegisterUpdateAsync(
                    _mapper.Map<Core.Entities.Logistic.InventoryCount>(before.Header),
                    _mapper.Map<Core.Entities.Logistic.InventoryCount>(after.Header),
                    auditLog,
                    transaction);

                foreach (var detail in _mapper.Map<List<InventoryCountDetail>>(after.Details))
                {
                    detail.BusinessId = businessId;
                    var detailAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.InventoryCountDetail,
                        detail.InventoryCountDetailId,
                        userId);

                    await _auditService.RegisterCreateAsync(detail, detailAuditLog, transaction);
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
            try { transaction.Rollback(); } catch { }
        }
    }
}
