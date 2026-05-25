using Application.DTOs.InventoryStock;
using Application.Exceptions;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;

namespace Application.UseCases.InventoryStock
{
    public class InventoryStockBusinessRules(
        IInventoryStockRepository repository,
        IProductsRepository productsRepository,
        IWarehousesRepository warehousesRepository)
    {
        private readonly IInventoryStockRepository _repository = repository;
        private readonly IProductsRepository _productsRepository = productsRepository;
        private readonly IWarehousesRepository _warehousesRepository = warehousesRepository;

        public async Task ValidateForCreateAsync(InventoryStockCreateDto dto, long businessId)
        {
            var product = await _productsRepository.GetByIdAsync(dto.ProductsId);
            if (product is null || product.BusinessId != businessId)
                throw new NotFoundException("Producto", dto.ProductsId);

            if (!product.IsActive)
                throw new BusinessException("El producto no esta activo.");

            if (!product.IsStockable)
                throw new BusinessException("El producto no es stockable.");

            var warehouse = await _warehousesRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse is null || warehouse.BusinessId != businessId)
                throw new NotFoundException("Almacen", dto.WarehouseId);

            if (await _repository.ExistsAsync(businessId, dto.WarehouseId, dto.ProductsId))
                throw new DuplicateEntryException("Ya existe un registro de stock para este producto en el almacen indicado.");

            if (dto.StockQuantity == 0 && (dto.AverageCost > 0 || dto.LastCost > 0))
                throw new BusinessException("No se puede registrar costo para un producto sin stock inicial.");
        }
    }
}
