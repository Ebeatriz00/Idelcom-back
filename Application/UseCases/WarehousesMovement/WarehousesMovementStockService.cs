using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;
using SharedKernel.Constants;
using System.Data;

namespace Application.UseCases.WarehousesMovement
{
    public class WarehousesMovementStockService(
        IInventoryStockRepository inventoryStockRepository,
        IInventoryKardexRepository inventoryKardexRepository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IInventoryStockRepository _inventoryStockRepository = inventoryStockRepository;
        private readonly IInventoryKardexRepository _inventoryKardexRepository = inventoryKardexRepository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task ApplyAsync(
            MovementResolution resolution,
            Core.Entities.Logistic.WarehousesMovement movement,
            WarehouseMovementDetail detail,
            long userId,
            IDbTransaction transaction)
        {
            switch (resolution.StockOperation)
            {
                case StockOperation.Entry:
                    await RegisterEntryStockAsync(
                        movement,
                        detail,
                        movement.WarehouseId,
                        detail.Quantity,
                        userId,
                        transaction);
                    break;

                case StockOperation.Output:
                    await RegisterOutputStockAsync(
                        movement,
                        detail,
                        movement.WarehouseId,
                        detail.Quantity,
                        resolution.AllowNegative,
                        userId,
                        transaction);
                    break;

                case StockOperation.Transfer:
                    await RegisterOutputStockAsync(
                        movement,
                        detail,
                        movement.WarehouseId,
                        detail.Quantity,
                        resolution.AllowNegative,
                        userId,
                        transaction);

                    await RegisterEntryStockAsync(
                        movement,
                        detail,
                        movement.WarehouseDestinationId,
                        detail.Quantity,
                        userId,
                        transaction);
                    break;
            }
        }

        private async Task RegisterEntryStockAsync(
            Core.Entities.Logistic.WarehousesMovement movement,
            WarehouseMovementDetail detail,
            long warehouseId,
            decimal quantity,
            long userId,
            IDbTransaction transaction)
        {
            var previousStock = await GetCurrentStockQuantityAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                transaction);

            await _inventoryStockRepository.IncreaseAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                quantity,
                detail.UnitCost,
                userId,
                transaction);

            var finalStock = await GetCurrentStockAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                transaction);

            await RegisterKardexAsync(
                movement,
                detail,
                warehouseId,
                entryQuantity: quantity,
                exitQuantity: 0,
                previousStock: previousStock,
                finalStock: finalStock,
                unitCost: detail.UnitCost,
                totalCost: detail.TotalCost,
                userId: userId,
                transaction: transaction);
        }

        private async Task RegisterOutputStockAsync(
            Core.Entities.Logistic.WarehousesMovement movement,
            WarehouseMovementDetail detail,
            long warehouseId,
            decimal quantity,
            bool allowNegative,
            long userId,
            IDbTransaction transaction)
        {
            var previousStock = await GetCurrentStockQuantityAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                transaction);

            await _inventoryStockRepository.DecreaseAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                quantity,
                userId,
                allowNegative,
                transaction);

            var finalStock = await GetCurrentStockAsync(
                movement.BusinessId,
                warehouseId,
                detail.ProductsId,
                transaction);

            await RegisterKardexAsync(
                movement,
                detail,
                warehouseId,
                entryQuantity: 0,
                exitQuantity: quantity,
                previousStock: previousStock,
                finalStock: finalStock,
                unitCost: finalStock?.AverageCost ?? 0,
                totalCost: quantity * (finalStock?.AverageCost ?? 0),
                userId: userId,
                transaction: transaction);
        }

        private async Task<decimal> GetCurrentStockQuantityAsync(
            long businessId,
            long warehouseId,
            long productsId,
            IDbTransaction transaction)
        {
            var stock = await GetCurrentStockAsync(businessId, warehouseId, productsId, transaction);
            return stock?.StockQuantity ?? 0;
        }

        private async Task<Core.Entities.Logistic.InventoryStock?> GetCurrentStockAsync(
            long businessId,
            long warehouseId,
            long productsId,
            IDbTransaction transaction)
        {
            return await _inventoryStockRepository.GetByProductAsync(
                businessId,
                warehouseId,
                productsId,
                transaction);
        }

        private async Task RegisterKardexAsync(
            Core.Entities.Logistic.WarehousesMovement movement,
            WarehouseMovementDetail detail,
            long warehouseId,
            decimal entryQuantity,
            decimal exitQuantity,
            decimal previousStock,
            Core.Entities.Logistic.InventoryStock? finalStock,
            decimal unitCost,
            decimal totalCost,
            long userId,
            IDbTransaction transaction)
        {
            if (finalStock is null)
                throw new DatabaseException("Error al registrar kardex de inventario.", "No se obtuvo el stock final.");

            var kardex = new InventoryKardex
            {
                BusinessId = movement.BusinessId,
                WarehouseId = warehouseId,
                ProductsId = detail.ProductsId,
                WareHouseMovementId = movement.WarehouseMovementId,
                WareHouseMovementDetailId = detail.WarehouseMovementDetailId,
                MovementTypesId = movement.MovementTypeId,
                MovementDate = movement.MovementDate ?? DateTime.Now,
                EntryQuantity = entryQuantity,
                ExitQuantity = exitQuantity,
                PreviousStock = previousStock,
                FinalStock = finalStock.StockQuantity,
                UnitCost = unitCost,
                AverageCost = finalStock.AverageCost,
                TotalCost = totalCost,
                ReferenceDocumentType = TableNames.WarehousesMovement,
                ReferenceDocumentNumber = BuildReferenceDocumentNumber(movement),
                Observation = detail.Observation ?? movement.Observation,
                CreateUser = userId
            };

            var created = await _inventoryKardexRepository.AddAsync(kardex, transaction);
            if (created.Id is null or <= 0)
                throw new DatabaseException("Error al registrar kardex de inventario.", "No se obtuvo el id creado.");

            kardex.InventoryKardexId = (long)created.Id;

            var auditLog = _auditLogFactory.Create(
                movement.BusinessId,
                TableNames.InventoryKardex,
                kardex.InventoryKardexId,
                userId);

            await _auditService.RegisterCreateAsync(kardex, auditLog, transaction);
        }

        private static string? BuildReferenceDocumentNumber(Core.Entities.Logistic.WarehousesMovement movement)
        {
            if (!string.IsNullOrWhiteSpace(movement.ReferenceDocument))
                return movement.ReferenceDocument;

            if (!string.IsNullOrWhiteSpace(movement.Series) && !string.IsNullOrWhiteSpace(movement.NumberDocument))
                return $"{movement.Series}-{movement.NumberDocument}";

            return movement.NumberDocument;
        }
    }
}
