using Application.DTOs.InventoryCount;
using AutoMapper;
using Core.Entities.paginations;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using FluentValidation;

namespace Application.UseCases.InventoryCount
{
    public class ListInventoryCountUseCase(
        IInventoryCountRepository repository,
        IValidator<InventoryCountListFilterDto> validator,
        IMapper mapper)
    {
        private readonly IInventoryCountRepository _repository = repository;
        private readonly IValidator<InventoryCountListFilterDto> _validator = validator;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<InventoryCountListResponseDto>> ExecuteAsync(InventoryCountListFilterDto dto, long businessId)
        {
            await InventoryCountValidation.ValidateAsync(_validator, dto);

            var filter = new InventoryCountFilter
            {
                BusinessId = businessId,
                WarehouseId = dto.WarehouseId,
                InventoryCountStatusId = dto.InventoryCountStatusId,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                Search = dto.Search,
                Page = dto.Page,
                PageSize = dto.PageSize
            };

            var result = await _repository.ListAsync(filter);

            return new PagedResult<InventoryCountListResponseDto>
            {
                Items = _mapper.Map<List<InventoryCountListResponseDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }
    }
}
