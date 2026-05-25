using Application.Exceptions;
using Application.UseCases.WarehousesMovement;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Interfaces;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using WarehouseEntity = Core.Entities.Logistic.WarehousesMovement;

namespace Application.UseCases.InventoryCount
{
    public class GenerateInventoryCountAdjustmentsUseCase(
        IInventoryCountRepository inventoryCountRepository,
        IWarehousesMovement movementRepository,
        WarehousesMovementStockService stockService,
        IMovementTypesRepository movementTypesRepository,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private const string PositiveAdjustmentCode = "009";
        private const string NegativeAdjustmentCode = "015";

        private readonly IInventoryCountRepository _inventoryCountRepository = inventoryCountRepository;
        private readonly IWarehousesMovement _movementRepository = movementRepository;
        private readonly WarehousesMovementStockService _stockService = stockService;
        private readonly IMovementTypesRepository _movementTypesRepository = movementTypesRepository;
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
                var before = await _inventoryCountRepository.GetByIdAsync(businessId, inventoryCountId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Toma de inventario", inventoryCountId);

                if (before.Header.StatusCode != "CLOSED")
                    throw new BusinessException("La toma de inventario debe estar cerrada para generar ajustes.");

                var detailsWithDiff = before.Details.Where(d => d.DifferenceQuantity != 0).ToList();
                if (detailsWithDiff.Count == 0)
                    throw new BusinessException("No existen diferencias para ajustar.");

                if (before.Details.Any(d => d.AdjustmentMovementId.HasValue))
                    throw new BusinessException("La toma de inventario ya fue ajustada.");

                var positiveDetails = detailsWithDiff.Where(d => d.DifferenceQuantity > 0).ToList();
                var negativeDetails = detailsWithDiff.Where(d => d.DifferenceQuantity < 0).ToList();

                var adjustments = new List<InventoryCountDetailAdjustmentCommand>();

                if (positiveDetails.Count > 0)
                {
                    var movType = await _movementTypesRepository.GetByCodeAsync(PositiveAdjustmentCode, businessId)
                        ?? throw new BusinessException($"No existe el tipo de movimiento '{PositiveAdjustmentCode}' activo para ajuste por sobrante.");

                    var movementId = await CreateAdjustmentMovementAsync(
                        before.Header, positiveDetails, movType, userId, businessId, transaction);

                    adjustments.AddRange(positiveDetails.Select(d => new InventoryCountDetailAdjustmentCommand
                    {
                        InventoryCountDetailId = d.InventoryCountDetailId,
                        AdjustmentMovementId = movementId
                    }));
                }

                if (negativeDetails.Count > 0)
                {
                    var movType = await _movementTypesRepository.GetByCodeAsync(NegativeAdjustmentCode, businessId)
                        ?? throw new BusinessException($"No existe el tipo de movimiento '{NegativeAdjustmentCode}' activo para ajuste por faltante.");

                    var movementId = await CreateAdjustmentMovementAsync(
                        before.Header, negativeDetails, movType, userId, businessId, transaction);

                    adjustments.AddRange(negativeDetails.Select(d => new InventoryCountDetailAdjustmentCommand
                    {
                        InventoryCountDetailId = d.InventoryCountDetailId,
                        AdjustmentMovementId = movementId
                    }));
                }

                var markCommand = new InventoryCountMarkAdjustedCommand
                {
                    BusinessId = businessId,
                    InventoryCountId = inventoryCountId,
                    UserId = userId,
                    Adjustments = adjustments
                };

                var result = await _inventoryCountRepository.MarkAsAdjustedAsync(markCommand, transaction);

                var after = await _inventoryCountRepository.GetByIdAsync(businessId, inventoryCountId, transaction);
                if (after?.Header is not null)
                {
                    var auditLog = _auditLogFactory.Create(
                        businessId, TableNames.InventoryCount, inventoryCountId, userId,
                        "Generacion de ajustes de inventario fisico");

                    await _auditService.RegisterUpdateAsync(
                        _mapper.Map<Core.Entities.Logistic.InventoryCount>(before.Header),
                        _mapper.Map<Core.Entities.Logistic.InventoryCount>(after.Header),
                        auditLog,
                        transaction);
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

        private async Task<long> CreateAdjustmentMovementAsync(
            InventoryCountHeaderProjection header,
            List<InventoryCountDetailProjection> details,
            Core.Entities.MovementTypes movType,
            long userId,
            long businessId,
            System.Data.IDbTransaction transaction)
        {
            var stockOperation = ResolveStockOperation(movType);
            var resolution = new MovementResolution(stockOperation, movType.AllowNegative, true);

            var movementEntity = new WarehouseEntity
            {
                BusinessId = businessId,
                MovementTypeId = movType.MovementTypesId,
                WarehouseId = header.WarehouseId,
                MovementDate = DateTime.Now,
                Observation = $"Ajuste por inventario fisico N° {header.CountNumber}",
                SourceModule = "INVENTORY_COUNT",
                SourceDocumentType = "INVENTORY_COUNT",
                SourceDocumentId = header.InventoryCountId,
                CreateUser = userId
            };

            var movementDetails = details.Select(d => new WarehouseMovementDetail
            {
                BusinessId = businessId,
                ProductsId = d.ProductsId,
                Quantity = Math.Abs(d.DifferenceQuantity),
                UnitCost = d.UnitCost,
                TotalCost = Math.Abs(d.DifferenceQuantity) * d.UnitCost,
                LotNumber = d.LotNumber,
                SerialNumber = d.SerialNumber,
                ExpirationDate = d.ExpirationDate,
                Observation = d.Observation,
                CreateUser = userId
            }).ToList();

            var created = await _movementRepository.AddAsync(movementEntity, movementDetails, transaction);
            if (created.Id is null or <= 0)
                throw new DatabaseException("Error al registrar el movimiento de ajuste.", "No se obtuvo el id creado.");

            movementEntity.WarehouseMovementId = (long)created.Id;

            var movAuditLog = _auditLogFactory.Create(
                businessId, TableNames.WarehousesMovement, movementEntity.WarehouseMovementId, userId);
            await _auditService.RegisterCreateAsync(movementEntity, movAuditLog, transaction);

            foreach (var detail in movementDetails)
            {
                detail.WarehouseMovementId = movementEntity.WarehouseMovementId;

                var createdDetail = await _movementRepository.AddDetailAsync(detail, transaction);
                if (createdDetail.Id is null or <= 0)
                    throw new DatabaseException("Error al registrar el detalle del ajuste.", "No se obtuvo el id creado.");

                detail.WarehouseMovementDetailId = (long)createdDetail.Id;

                var detailAuditLog = _auditLogFactory.Create(
                    businessId, TableNames.WarehousesMovementDetail, detail.WarehouseMovementDetailId, userId);
                await _auditService.RegisterCreateAsync(detail, detailAuditLog, transaction);

                await _stockService.ApplyAsync(resolution, movementEntity, detail, userId, transaction);
            }

            return movementEntity.WarehouseMovementId;
        }

        private static StockOperation ResolveStockOperation(Core.Entities.MovementTypes movType)
        {
            if (movType.RequiresDestWare)
                return StockOperation.Transfer;

            return movType.MovOperId switch
            {
                1 => StockOperation.Entry,
                2 => StockOperation.Output,
                _ => throw new BusinessException("No se pudo determinar si el tipo de movimiento de ajuste es entrada o salida.")
            };
        }

        private static void TryRollback(System.Data.IDbTransaction transaction)
        {
            try { transaction.Rollback(); } catch { }
        }
    }
}
