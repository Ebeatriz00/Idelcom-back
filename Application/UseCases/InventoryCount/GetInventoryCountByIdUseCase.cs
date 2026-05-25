using Application.DTOs.InventoryCount;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Logistic;

namespace Application.UseCases.InventoryCount
{
    public class GetInventoryCountByIdUseCase(
        IInventoryCountRepository repository,
        IMapper mapper)
    {
        private readonly IInventoryCountRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<InventoryCountFullResponseDto> ExecuteAsync(long businessId, long inventoryCountId)
        {
            var result = await _repository.GetByIdAsync(businessId, inventoryCountId);
            if (result?.Header is null)
                throw new NotFoundException("Toma de inventario", inventoryCountId);

            return new InventoryCountFullResponseDto
            {
                Header = _mapper.Map<InventoryCountListResponseDto>(result.Header),
                Details = _mapper.Map<List<InventoryCountDetailResponseDto>>(result.Details)
            };
        }
    }
}
