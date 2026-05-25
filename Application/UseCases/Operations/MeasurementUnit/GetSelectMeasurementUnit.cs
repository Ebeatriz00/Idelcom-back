using Application.DTOs.Operations.MeasurementUnit;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.MeasurementUnit
{
    public class GetSelectMeasurementUnit(IMeasurementUnitRepository repository, IMapper mapper)
    {
        private readonly IMeasurementUnitRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<MeasurementUnitSelectDto>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<MeasurementUnitSelectDto>>(result);
        }
    }
}
