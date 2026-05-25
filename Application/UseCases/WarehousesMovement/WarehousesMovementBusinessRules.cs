using Application.DTOs.WarehousesMovement;
using Application.Exceptions;
using Core.Interfaces;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;

namespace Application.UseCases.WarehousesMovement
{
    public class WarehousesMovementBusinessRules(
        IWarehousesRepository warehousesRepository,
        IProductsRepository productsRepository,
        IMovementTypesRepository movementTypesRepository)
    {
        private readonly IWarehousesRepository _warehousesRepository = warehousesRepository;
        private readonly IProductsRepository _productsRepository = productsRepository;
        private readonly IMovementTypesRepository _movementTypesRepository = movementTypesRepository;

        public async Task<MovementResolution> ValidateForCreateAsync(WarehousesMovementCreateDto dto, long businessId)
        {
            var movementType = await _movementTypesRepository.GetByIdAsync(dto.MovementTypeId);
            if (movementType is null || movementType.BusinessId != businessId)
                throw new NotFoundException("Tipo de movimiento", dto.MovementTypeId);

            if (movementType.Status != "1")
                throw new BusinessException("El tipo de movimiento no esta activo.");

            if (!movementType.AffectsStock)
                throw new BusinessException("El tipo de movimiento debe afectar stock para registrar movimientos de almacen.");

            if (movementType.IsAdjustment && string.IsNullOrWhiteSpace(dto.Observation))
                throw new BusinessException("Debe ingresar un motivo para el ajuste de inventario.");

            var stockOperation = ResolveStockOperation(movementType);
            await ValidateWarehousesAsync(dto, businessId, stockOperation, movementType.RequiresDestWare);

            var duplicateProducts = dto.Details
                .GroupBy(d => d.ProductsId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateProducts.Count > 0)
                throw new BusinessException("No se permite repetir productos en el detalle del movimiento.");

            foreach (var detail in dto.Details)
            {
                var product = await _productsRepository.GetByIdAsync(detail.ProductsId);
                if (product is null || product.BusinessId != businessId)
                    throw new NotFoundException("Producto", detail.ProductsId);

                if (!product.IsActive)
                    throw new BusinessException($"El producto {detail.ProductsId} no esta activo.");

                if (!product.IsStockable)
                    throw new BusinessException($"El producto {detail.ProductsId} no es stockable.");

                if (product.ManageLots && string.IsNullOrWhiteSpace(detail.LotNumber))
                    throw new BusinessException($"El producto {detail.ProductsId} requiere numero de lote.");

                if (product.ManegesSerials && string.IsNullOrWhiteSpace(detail.SerialNumber))
                    throw new BusinessException($"El producto {detail.ProductsId} requiere numero de serie.");

                if (product.ExpirationControl && !detail.ExpirationDate.HasValue)
                    throw new BusinessException($"El producto {detail.ProductsId} requiere fecha de caducidad.");
            }

            return new MovementResolution(stockOperation, movementType.AllowNegative, movementType.IsAdjustment);
        }

        private async Task ValidateWarehousesAsync(
            WarehousesMovementCreateDto dto,
            long businessId,
            StockOperation stockOperation,
            bool requiresDestinationWarehouse)
        {
            var hasOriginWarehouse = dto.WarehouseId > 0;
            var destinationWarehouseId = dto.WarehouseDestinationId.GetValueOrDefault();
            var hasDestinationWarehouse = destinationWarehouseId > 0;

            if (requiresDestinationWarehouse && !hasDestinationWarehouse)
                throw new BusinessException("Este tipo de movimiento requiere almacen destino.");

            switch (stockOperation)
            {
                case StockOperation.Entry:
                    if (!hasOriginWarehouse)
                        throw new BusinessException("Los movimientos de entrada requieren almacen.");
                    break;

                case StockOperation.Output:
                    if (!hasOriginWarehouse)
                        throw new BusinessException("Los movimientos de salida requieren almacen.");
                    break;

                case StockOperation.Transfer:
                    if (!hasOriginWarehouse)
                        throw new BusinessException("Los movimientos de transferencia requieren almacen origen.");

                    if (!hasDestinationWarehouse)
                        throw new BusinessException("Los movimientos de transferencia requieren almacen destino.");
                    break;
            }

            if (hasOriginWarehouse && hasDestinationWarehouse && dto.WarehouseId == destinationWarehouseId)
                throw new BusinessException("El almacen origen y destino no pueden ser el mismo.");

            if (hasOriginWarehouse)
                await EnsureWarehouseExistsAsync(dto.WarehouseId, businessId, "almacen origen");

            if (hasDestinationWarehouse)
                await EnsureWarehouseExistsAsync(destinationWarehouseId, businessId, "almacen destino");
        }

        private async Task EnsureWarehouseExistsAsync(long warehouseId, long businessId, string label)
        {
            var warehouse = await _warehousesRepository.GetByIdAsync(warehouseId);
            if (warehouse is null || warehouse.BusinessId != businessId)
                throw new NotFoundException(label, warehouseId);
        }

        // MOV_OPER_ID = 1 → Entry, MOV_OPER_ID = 2 → Output. REQUIRES_DEST_WARE overrides to Transfer.
        private static StockOperation ResolveStockOperation(Core.Entities.MovementTypes movementType)
        {
            if (movementType.RequiresDestWare)
                return StockOperation.Transfer;

            return movementType.MovOperId switch
            {
                1 => StockOperation.Entry,
                2 => StockOperation.Output,
                _ => throw new BusinessException("No se pudo determinar si el tipo de movimiento es entrada, salida o transferencia.")
            };
        }
    }
}
