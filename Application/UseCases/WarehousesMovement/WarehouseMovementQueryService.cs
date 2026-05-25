using Application.DTOs.WarehousesMovement;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;

namespace Application.UseCases.WarehousesMovement
{
    public class WarehouseMovementQueryService(IWarehousesMovement repository, IMapper mapper)
    {
        private readonly IWarehousesMovement _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<WarehouseMovementListDto>> ListAsync(long businessId, WarehouseMovementFilterDto filter)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var repositoryFilter = new WarehouseMovementFilter
            {
                BusinessId = businessId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = filter.Search,
                MovementTypeId = NormalizeId(filter.MovementTypeId),
                MovOperId = NormalizeId(filter.MovOperId),
                WarehouseId = NormalizeId(filter.WarehouseId),
                DateFrom = filter.DateFrom,
                DateTo = filter.DateTo
            };

            var result = await _repository.ListAsync(repositoryFilter);
            return _mapper.Map<PagedResult<WarehouseMovementListDto>>(result);
        }

        public async Task<WarehouseMovementByIdDto> GetByIdAsync(long businessId, long warehouseMovementId)
        {
            var result = await _repository.GetByIdAsync(businessId, warehouseMovementId);
            if (result is null || result.Header is null)
                throw new NotFoundException("Movimiento de almacen", warehouseMovementId);

            var dto = _mapper.Map<WarehouseMovementByIdDto>(result.Header);
            dto.Details = _mapper.Map<IReadOnlyList<WarehouseMovementDetailDto>>(result.Details);
            return dto;
        }

        public async Task<IReadOnlyList<InventoryStockAvailableDto>> GetAvailableStockAsync(
            long businessId,
            InventoryStockAvailableFilterDto filter)
        {
            if (filter.WarehouseId <= 0)
                throw new BusinessException("El almacen es obligatorio.");

            var repositoryFilter = new InventoryStockAvailableFilter
            {
                BusinessId = businessId,
                WarehouseId = filter.WarehouseId,
                ProductsId = NormalizeId(filter.ProductsId),
                Search = filter.Search
            };

            var result = await _repository.GetAvailableStockAsync(repositoryFilter);
            return _mapper.Map<IReadOnlyList<InventoryStockAvailableDto>>(result);
        }

        private static long? NormalizeId(long? value)
        {
            return value.HasValue && value.Value > 0 ? value.Value : null;
        }
    }
}
