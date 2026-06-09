using Application.DTOs.Ssoma.Clinics;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma.Clinics;

namespace Application.UseCases.Ssoma.Clinics
{
    public class GetAllClinics(IClinicRepository repository, IMapper mapper)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<ClinicResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var result = await _repository.GetAllAsync(businessId, search, page, pageSize);

            return new PagedResult<ClinicResponseDto>
            {
                Items = _mapper.Map<List<ClinicResponseDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }
    }
}
